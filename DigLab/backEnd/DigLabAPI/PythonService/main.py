# backEnd/DigLabAPI/PythonService/main.py
from __future__ import annotations

from fastapi import FastAPI, UploadFile, File, HTTPException, Query
from fastapi.responses import FileResponse
from pydantic import BaseModel, Field
from typing import List, Optional
from pathlib import Path
from datetime import datetime
import re, uuid, csv, qrcode
import fitz  # PyMuPDF

from pdf_form import generate_lab_form_pdf

# -----------------------------------------------------------------------------
# App (ONE instance!)
# -----------------------------------------------------------------------------
app = FastAPI(title="DigLab PyService", version="1.1.0")

# -----------------------------------------------------------------------------
# Config / constants
# -----------------------------------------------------------------------------
DIAGNOSES = ["Dengue", "Malaria", "TBE", "Hantavirus – Puumalavirus (PuV)"]
LABNUM_RE = re.compile(r"LAB-\d{8}-[A-Z0-9]{8}")

BASE_DIR = Path(__file__).parent
CSV_PATH = BASE_DIR / "samples.csv"
BARCODES_DIR = BASE_DIR / "barcodes"
FORMS_DIR = BASE_DIR / "forms"
BARCODES_DIR.mkdir(parents=True, exist_ok=True)
FORMS_DIR.mkdir(parents=True, exist_ok=True)

# storage folders (python side)
RESULTS_DIR = BASE_DIR / "formResults"
RESULTS_DIR.mkdir(parents=True, exist_ok=True)


# -----------------------------------------------------------------------------
# Helpers
# -----------------------------------------------------------------------------
def ensure_csv():
    if not CSV_PATH.exists():
        with CSV_PATH.open("w", newline="", encoding="utf-8") as f:
            csv.writer(f).writerow(
                ["labnummer", "personnummer", "date", "time", "created_at"]
            )

def make_labnummer(date_iso: str) -> str:
    ymd = date_iso.replace("-", "")
    return f"LAB-{ymd}-{uuid.uuid4().hex[:8].upper()}"

def append_csv(labnummer: str, personnummer: str, date_iso: str, time_hm: str):
    ensure_csv()
    with CSV_PATH.open("a", newline="", encoding="utf-8") as f:
        csv.writer(f).writerow(
            [labnummer, personnummer, date_iso, time_hm, datetime.utcnow().isoformat()]
        )

def read_rows():
    if not CSV_PATH.exists():
        return []
    with CSV_PATH.open("r", newline="", encoding="utf-8") as f:
        return list(csv.DictReader(f))

def find_by_labnummer(lab: str):
    for r in read_rows():
        if r.get("labnummer", "").strip().upper() == lab.strip().upper():
            return r
    return None

def find_by_personnummer(pp: str):
    for r in read_rows():
        if r.get("personnummer", "").strip() == pp.strip():
            return r
    return None

def make_qr_png(data: str) -> Path:
    out = BARCODES_DIR / f"{data}.png"
    qrcode.make(data).save(out)
    return out

def extract_text_from_pdf_bytes(pdf_bytes: bytes) -> str:
    text = []
    with fitz.open(stream=pdf_bytes, filetype="pdf") as doc:
        for page in doc:
            text.append(page.get_text("text"))
    return "\n".join(text)

def parse_fields_from_text(text: str) -> dict:
    def grab(pat: str):
        m = re.search(pat, text, re.IGNORECASE)
        return m.group(1).strip() if m else None

    name = grab(r"Full Name:\s*(.+)")
    pnr  = grab(r"Personnummer:\s*([0-9]{11})")
    date = grab(r"Date:\s*([0-9]{4}-[0-9]{2}-[0-9]{2})")
    time = grab(r"Time:\s*([0-9]{2}:[0-9]{2})")

    found_dx: List[str] = []
    for d in DIAGNOSES:
        if re.search(rf"{re.escape(d)}\s*\[\s*X\s*\]", text, re.IGNORECASE):
            found_dx.append(d)

    return {
        "name": name,
        "personnummer": pnr,
        "date": date,
        "time": time,
        "diagnoses": found_dx or None,
    }

def compute_overall(marks: dict[str, str], requested_only: set[str] | None = None) -> str:
    items = ((d, v) for d, v in marks.items() if requested_only is None or d in requested_only)
    vals = [v for _, v in items]
    has_pos = any(v == "positive" for v in vals)
    has_neg = any(v == "negative" for v in vals)
    if has_pos and not has_neg:
        return "positive"
    if has_neg and not has_pos:
        return "negative"
    if has_pos and has_neg:
        return "mixed"
    return "inconclusive"



# -----------------------------------------------------------------------------
# Schemas
# -----------------------------------------------------------------------------
class RegisterRequest(BaseModel):
    personnummer: str = Field(..., min_length=11, max_length=11)
    date: str
    time: str

class BarcodeRequest(BaseModel):
    personnummer: Optional[str] = None
    labnummer: Optional[str] = None
    date: Optional[str] = None  # fallback if labnummer must be generated

class GenerateFormRequest(BaseModel):
    name: str
    date: str
    time: str
    diagnoses: List[str] = Field(default_factory=list)
    personnummer: Optional[str] = None
    labnummer: Optional[str] = None
    qr_data: Optional[str] = None  # explicit QR payload

# -----------------------------------------------------------------------------
# Routes
# -----------------------------------------------------------------------------
@app.get("/")
def root():
    return {
        "service": "DigLab PyService",
        "endpoints": ["/health", "/register", "/barcode", "/lookup", "/generate-form", "/analyze"],
    }

@app.get("/health")
def health():
    return {"status": "ok", "time": datetime.utcnow().isoformat()}

@app.post("/register")
def register(req: RegisterRequest):
    if not req.personnummer.isdigit() or len(req.personnummer) != 11:
        raise HTTPException(status_code=400, detail="personnummer must be 11 digits")
    try:
        datetime.strptime(req.date, "%Y-%m-%d")
        datetime.strptime(req.time, "%H:%M")
    except ValueError:
        raise HTTPException(status_code=400, detail="Invalid date/time format")

    labnummer = make_labnummer(req.date)
    append_csv(labnummer, req.personnummer, req.date, req.time)
    return {"labnummer": labnummer, "personnummer": req.personnummer, "date": req.date, "time": req.time}

@app.post("/barcode")
def barcode(req: BarcodeRequest):
    if req.personnummer:
        payload = req.personnummer
    elif req.labnummer:
        payload = req.labnummer
    else:
        if not req.date:
            raise HTTPException(status_code=400, detail="Provide personnummer or labnummer or date")
        payload = make_labnummer(req.date)

    png_path = make_qr_png(payload)
    return {"qr_data": payload, "png": str(png_path)}

@app.get("/lookup")
def lookup(labnummer: Optional[str] = Query(None), personnummer: Optional[str] = Query(None)):
    if labnummer:
        row = find_by_labnummer(labnummer)
    elif personnummer:
        row = find_by_personnummer(personnummer)
    else:
        raise HTTPException(status_code=400, detail="Provide labnummer or personnummer")

    if not row:
        raise HTTPException(status_code=404, detail="Not found")

    return row

@app.post("/generate-form")
def generate_form(req: GenerateFormRequest):
    try:
        datetime.strptime(req.date, "%Y-%m-%d")
        datetime.strptime(req.time, "%H:%M")
    except ValueError:
        raise HTTPException(status_code=400, detail="Invalid date/time format")

    labnummer = req.labnummer or make_labnummer(req.date)
    qr_payload = (req.qr_data or labnummer).strip()
    qr_png = make_qr_png(qr_payload)

    pdf_path = FORMS_DIR / f"{labnummer}.pdf"
    generate_lab_form_pdf(
        out_pdf=pdf_path,
        labnummer=labnummer,
        name=req.name,
        date=req.date,
        time=req.time,
        diagnoses=req.diagnoses,
        qr_png=qr_png,
        personnummer=req.personnummer,
    )
    return FileResponse(pdf_path, media_type="application/pdf", filename=pdf_path.name)


import numpy as np
from PIL import Image

def analyze_typed_marks(text: str) -> dict[str, str]:
    """
    Parse rows like: 'Dengue [X] [ ] [ ]'  -> req, pos, neg
    Returns: {'Dengue': 'positive'|'negative'|'none', ...}
    """
    marks: dict[str, str] = {}
    for d in DIAGNOSES:
        # capture three bracket cells in order: Requested, Positive, Negative
        m = re.search(
            rf"{re.escape(d)}\s*\[\s*([Xx]?)\s*\]\s*\[\s*([Xx]?)\s*\]\s*\[\s*([Xx]?)\s*\]",
            text, re.IGNORECASE
        )
        if not m:
            continue
        pos_x = m.group(2).strip().upper() == "X"
        neg_x = m.group(3).strip().upper() == "X"
        marks[d] = "positive" if pos_x else ("negative" if neg_x else "none")
    return marks

import numpy as np
from PIL import Image

def analyze_pen_marks_from_pdf(
    pdf_bytes: bytes,
    requested_only: set[str] | None = None
) -> dict[str, str]:
    """
    Detect pen marks tightly around the printed checkbox token.
    Only evaluates rows in `requested_only` if provided.
    """
    marks: dict[str, str] = {}

    with fitz.open(stream=pdf_bytes, filetype="pdf") as doc:
        page = doc[0]

        # Render WITH ink annotations
        scale = 300 / 72.0
        pix = page.get_pixmap(matrix=fitz.Matrix(scale, scale), alpha=False, annots=True)
        img = Image.frombytes("RGB", [pix.width, pix.height], pix.samples)

        def rect_to_px(r: fitz.Rect):
            return (int(r.x0 * scale), int(r.y0 * scale), int(r.x1 * scale), int(r.y1 * scale))

        def row_mid_y(r: fitz.Rect) -> float:
            return 0.5 * (r.y0 + r.y1)

        # Prefer the redesigned token "[    ]" (four spaces)
        box_rects: list[fitz.Rect] = []
        for token in ("[    ]", "[   ]", "[  ]", "[ ]", "[X]", "[x]"):
            box_rects += page.search_for(token)

        # Tight ROI knobs (PDF points)
        row_tol   = 12.0
        pad_x     = 4.0
        pad_y     = 6.0
        widen_mul = 1.25
        dark_thr  = 0.08     # pen strokes are thin → keep low-ish
        ratio_win = 1.25

        def tight_roi(box: fitz.Rect) -> fitz.Rect:
            w = box.width * widen_mul
            cx = 0.5 * (box.x0 + box.x1)
            x0, x1 = cx - 0.5 * w, cx + 0.5 * w
            y0, y1 = box.y0 - pad_y, box.y1 + pad_y
            return fitz.Rect(x0 - pad_x, y0, x1 + pad_x, y1)

        def dark_fraction(r: fitz.Rect) -> float:
            x0, y0, x1, y1 = rect_to_px(r)
            x0 = max(x0, 0); y0 = max(y0, 0)
            x1 = min(x1, img.width); y1 = min(y1, img.height)
            if x1 <= x0 or y1 <= y0:
                return 0.0
            gray = img.crop((x0, y0, x1, y1)).convert("L")
            arr  = np.asarray(gray, dtype=np.uint8)
            return (arr < 200).mean()

        for d in DIAGNOSES:
            # Skip unrequested rows entirely (force to "none")
            if requested_only is not None and d not in requested_only:
                marks[d] = "none"
                continue

            hits = page.search_for(d)
            if not hits:
                marks[d] = "none"
                continue

            cy = row_mid_y(hits[0])
            row_boxes = [r for r in box_rects if abs(row_mid_y(r) - cy) <= row_tol]
            if len(row_boxes) < 3:
                marks[d] = "none"
                continue

            # Keep 3 most aligned; sort left->right => [requested, positive, negative]
            row_boxes.sort(key=lambda r: abs(row_mid_y(r) - cy))
            row_boxes = row_boxes[:3]
            row_boxes.sort(key=lambda r: r.x0)
            _, pos_box, neg_box = row_boxes

            df_pos = dark_fraction(tight_roi(pos_box))
            df_neg = dark_fraction(tight_roi(neg_box))

            if df_pos < dark_thr and df_neg < dark_thr:
                marks[d] = "none"
            elif df_pos >= df_neg * ratio_win:
                marks[d] = "positive"
            elif df_neg >= df_pos * ratio_win:
                marks[d] = "negative"
            else:
                marks[d] = "positive" if df_pos >= df_neg else "negative"

    return marks







@app.post("/analyze")
async def analyze(file: UploadFile = File(...)):
    if not file:
        raise HTTPException(status_code=400, detail="No file uploaded")

    content = await file.read()
    ctype = (file.content_type or "").lower()
    if not ("pdf" in ctype or file.filename.lower().endswith(".pdf")):
        raise HTTPException(status_code=415, detail=f"Unsupported content type: {ctype}")

    # 1) Extract text for metadata + requested list (Requested column has [X])
    text = extract_text_from_pdf_bytes(content)

    labnummer = None
    m = LABNUM_RE.search(text)
    if m:
        labnummer = m.group(0)

    found = parse_fields_from_text(text)
    requested: set[str] = set(found.get("diagnoses") or [])  # ONLY these rows count

    # 2) Detect pen marks ONLY on requested rows
    pen = analyze_pen_marks_from_pdf(content, requested_only=requested)

    # 3) Build a marks dict for all rows, but force unrequested -> "none"
    marks = {d: (pen.get(d, "none") if d in requested else "none") for d in DIAGNOSES}

    # 4) Overall result from requested rows only
    overall = compute_overall(marks, requested_only=requested)

    return {
        "labnummer": labnummer,
        "result": overall,      # positive | negative | mixed | inconclusive
        "confidence": 0.0,
        "found": found,         # includes requested diagnoses list
        "marks": marks          # unrequested rows are "none"
    }




from fastapi.responses import Response
from pydantic import BaseModel
import fitz  # PyMuPDF

class FinalizeRow(BaseModel):
    diagnosis: str
    final: str
    auto: str | None = None

class FinalizePayload(BaseModel):
    labnummer: str
    results: list[FinalizeRow]

@app.post("/finalize-form")
def finalize_form(payload: FinalizePayload):
    """
    Load the requisition PDF for `labnummer` from FORMS_DIR, stamp FINAL results,
    return the updated PDF as bytes (and also save a copy on the Python side).
    """
    # Your /generate-form saved as "<labnummer>.pdf"
    src = FORMS_DIR / f"{payload.labnummer}.pdf"
    if not src.exists():
        # also try a couple of fallback names if you ever change naming
        alt = FORMS_DIR / f"DigLab-{payload.labnummer}.pdf"
        if alt.exists():
            src = alt
        else:
            raise HTTPException(status_code=404, detail=f"Requisition PDF not found for {payload.labnummer}")

    doc = fitz.open(src)
    page = doc[0]

    # Simple stamp layout
    x_left = 54
    y = 130
    line = 18

    page.insert_text((x_left, y), "FINAL RESULTS", fontsize=14, fontname="helv", fill=(0, 0, 0))
    y += line + 6

    # Sort for stable order
    for r in sorted(payload.results, key=lambda r: r.diagnosis.lower()):
        txt = f"{r.diagnosis}: {r.final}"
        page.insert_text((x_left, y), txt, fontsize=12, fontname="helv", fill=(0, 0, 0))
        y += line

    out_bytes = doc.tobytes()
    doc.close()

    # Optional: also store a copy on the Python side (not required for .NET)
    (RESULTS_DIR / f"DigLab-{payload.labnummer}-results.pdf").write_bytes(out_bytes)

    return Response(content=out_bytes, media_type="application/pdf")



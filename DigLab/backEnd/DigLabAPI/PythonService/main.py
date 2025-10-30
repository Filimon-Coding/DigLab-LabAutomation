# backEnd/DigLabAPI/PythonService/main.py
from __future__ import annotations

from datetime import datetime
from pathlib import Path
from typing import List, Optional

import csv
import qrcode
import re
import uuid

import fitz  # PyMuPDF
import numpy as np
from PIL import Image

from fastapi import FastAPI, File, HTTPException, Query, UploadFile
from fastapi.responses import FileResponse, Response
from pydantic import BaseModel, Field

from pdf_form import generate_lab_form_pdf


# App

app = FastAPI(title="DigLab PyService", version="1.2.0")

# Config / constants

DIAGNOSES = ["Dengue", "Malaria", "TBE", "Hantavirus – Puumalavirus (PuV)"]
LABNUM_RE = re.compile(r"LAB-\d{8}-[A-Z0-9]{8}")

BASE_DIR = Path(__file__).parent

CSV_PATH     = BASE_DIR / "samples.csv"
BARCODES_DIR = BASE_DIR / "barcodes"
FORMS_DIR    = BASE_DIR / "forms"        # requisition PDFs from /generate-form
RESULTS_DIR  = BASE_DIR / "formResults"  # finalized PDFs (stamped)
SCANS_DIR    = BASE_DIR / "scans"        # uploaded/pen-marked PDFs from /analyze

for d in (BARCODES_DIR, FORMS_DIR, RESULTS_DIR, SCANS_DIR):
    d.mkdir(parents=True, exist_ok=True)

# -----------------------------------------------------------------------------
# Helpers
# -----------------------------------------------------------------------------
def ensure_csv() -> None:
    if not CSV_PATH.exists():
        with CSV_PATH.open("w", newline="", encoding="utf-8") as f:
            csv.writer(f).writerow(["labnummer", "personnummer", "date", "time", "created_at"])

def make_labnummer(date_iso: str) -> str:
    ymd = date_iso.replace("-", "")
    return f"LAB-{ymd}-{uuid.uuid4().hex[:8].upper()}"

def append_csv(labnummer: str, personnummer: str, date_iso: str, time_hm: str) -> None:
    ensure_csv()
    with CSV_PATH.open("a", newline="", encoding="utf-8") as f:
        csv.writer(f).writerow([labnummer, personnummer, date_iso, time_hm, datetime.utcnow().isoformat()])

def read_rows() -> list[dict]:
    if not CSV_PATH.exists():
        return []
    with CSV_PATH.open("r", newline="", encoding="utf-8") as f:
        return list(csv.DictReader(f))

def find_by_labnummer(lab: str) -> Optional[dict]:
    lab = lab.strip().upper()
    return next((r for r in read_rows() if r.get("labnummer", "").strip().upper() == lab), None)

def find_by_personnummer(pp: str) -> Optional[dict]:
    pp = pp.strip()
    return next((r for r in read_rows() if r.get("personnummer", "").strip() == pp), None)

def make_qr_png(data: str) -> Path:
    out = BARCODES_DIR / f"{data}.png"
    qrcode.make(data).save(out)
    return out

def extract_text_from_pdf_bytes(pdf_bytes: bytes) -> str:
    parts: list[str] = []
    with fitz.open(stream=pdf_bytes, filetype="pdf") as doc:
        for page in doc:
            parts.append(page.get_text("text"))
    return "\n".join(parts)

def parse_fields_from_text(text: str) -> dict:
    def grab(pat: str) -> Optional[str]:
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

    return {"name": name, "personnummer": pnr, "date": date, "time": time, "diagnoses": found_dx or None}

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

def analyze_pen_marks_from_pdf(pdf_bytes: bytes, requested_only: set[str] | None = None) -> dict[str, str]:
    """
    Detect pen marks near the Positive/Negative boxes per diagnosis.
    Only evaluates rows in `requested_only` if provided.
    """
    marks: dict[str, str] = {}

    with fitz.open(stream=pdf_bytes, filetype="pdf") as doc:
        page = doc[0]

        # Render WITH ink annotations
        scale = 300 / 72.0
        pix = page.get_pixmap(matrix=fitz.Matrix(scale, scale), alpha=False, annots=True)
        img = Image.frombytes("RGB", [pix.width, pix.height], pix.samples)

        def rect_to_px(r: fitz.Rect) -> tuple[int, int, int, int]:
            return int(r.x0 * scale), int(r.y0 * scale), int(r.x1 * scale), int(r.y1 * scale)

        def row_mid_y(r: fitz.Rect) -> float:
            return 0.5 * (r.y0 + r.y1)

        # Tokens used in the form for empty/checked boxes
        token_candidates = ("[    ]", "[   ]", "[  ]", "[ ]", "[X]", "[x]")
        box_rects: list[fitz.Rect] = []
        for token in token_candidates:
            box_rects += page.search_for(token)

        # ROI and threshold knobs (units: PDF points unless otherwise stated)
        row_tol   = 12.0
        pad_x     = 4.0
        pad_y     = 6.0
        widen_mul = 1.25
        dark_thr  = 0.08
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

            # Choose the 3 most aligned; left->right = [Requested, Positive, Negative]
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

# -----------------------------------------------------------------------------
# Schemas
# -----------------------------------------------------------------------------
class RegisterRequest(BaseModel):
    personnummer: str = Field(..., min_length=11, max_length=11)
    date: str
    time: str

class BarcodeRequest(BaseModel):
    personnummer: Optional[str] = None
    labnummer: Optional[str]    = None
    date: Optional[str]         = None  # fallback if labnummer must be generated

class GenerateFormRequest(BaseModel):
    name: str
    date: str
    time: str
    diagnoses: List[str] = Field(default_factory=list)
    personnummer: Optional[str] = None
    labnummer: Optional[str]    = None
    qr_data: Optional[str]      = None  # explicit QR payload

class FinalizeRow(BaseModel):
    diagnosis: str
    final: str
    auto: Optional[str] = None

class FinalizePayload(BaseModel):
    labnummer: str
    results: List[FinalizeRow]

# -----------------------------------------------------------------------------
# Routes
# -----------------------------------------------------------------------------
@app.get("/")
def root():
    return {
        "service": "DigLab PyService",
        "endpoints": [
            "/health",
            "/register",
            "/barcode",
            "/lookup",
            "/generate-form",
            "/analyze",
            "/finalize-form",
        ],
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

@app.post("/analyze")
async def analyze(file: UploadFile = File(...)):
    if not file:
        raise HTTPException(status_code=400, detail="No file uploaded")

    content = await file.read()
    ctype = (file.content_type or "").lower()
    if not ("pdf" in ctype or file.filename.lower().endswith(".pdf")):
        raise HTTPException(status_code=415, detail=f"Unsupported content type: {ctype}")

    # Extract text to find labnummer & requested diagnoses
    text = extract_text_from_pdf_bytes(content)

    labnummer = None
    m = LABNUM_RE.search(text)
    if m:
        labnummer = m.group(0)
        # Save the *original scanned* PDF – we’ll stamp on this in /finalize-form
        (SCANS_DIR / f"{labnummer}.pdf").write_bytes(content)

    found = parse_fields_from_text(text)
    requested: set[str] = set(found.get("diagnoses") or [])  # ONLY requested rows count

    # Detect pen marks ONLY in requested rows
    pen = analyze_pen_marks_from_pdf(content, requested_only=requested)

    # Build marks for all rows (unrequested -> "none")
    marks = {d: (pen.get(d, "none") if d in requested else "none") for d in DIAGNOSES}

    overall = compute_overall(marks, requested_only=requested)

    return {
        "labnummer": labnummer,
        "result": overall,
        "confidence": 0.0,
        "found": found,
        "marks": marks,
        "scanSaved": bool(labnummer and (SCANS_DIR / f"{labnummer}.pdf").exists()),
    }



def _find_signature_anchor(doc: fitz.Document) -> tuple[int, float | None]:
    """
    Finn siden og Y-posisjonen (underste kant) for signatur-seksjonen.
    Returnerer (page_index, anchor_y). Hvis ikke funnet: (0, None).
    """
    best_page = 0
    best_y = None

    terms = ["Received by:", "Collected by:", "Signatures"]
    for i in range(len(doc)):
        page = doc[i]
        y_hits: list[float] = []
        for t in terms:
            rects = page.search_for(t)
            if rects:
                y_hits.append(max(r.y1 for r in rects))  # nederste kant av teksten
        if y_hits:
            y = max(y_hits)
            # velg «seneste» forekomst (senere side vinner; ved lik side vinner høyere y)
            if best_y is None or i > best_page or (i == best_page and y > best_y):
                best_page, best_y = i, y

    return best_page, best_y


@app.post("/finalize-form")
def finalize_form(payload: FinalizePayload):
    """
    Stamp FINAL RESULTS on the scanned PDF (preferred) or the requisition.
    Text is red + bold-ish and placed ~3 px below the signature table.
    """
    lab = payload.labnummer.strip()

    # Prefer pen-marked scan; fall back to requisition
    src = SCANS_DIR / f"{lab}.pdf"
    if not src.exists():
        candidates = [FORMS_DIR / f"{lab}.pdf", FORMS_DIR / f"DigLab-{lab}.pdf"]
        src = next((p for p in candidates if p.exists()), None)
        if not src:
            raise HTTPException(status_code=404, detail=f"Source PDF not found for {lab}")

    doc = fitz.open(src)

    # --- find signature anchor (page + bottom Y of the signatures block)
    page_idx, anchor_y = _find_signature_anchor(doc)
    page = doc[page_idx]

    # Layout
    left  = 54.0
    line  = 18.0
    pad   = 20.0        # <- only 3 points (~20 px) below the signature box
    bottom_margin = 36.0

    header_size = 16
    row_size    = 13

    # total block height (header + rows + small padding)
    block_h = 28 + len(payload.results) * line + 10

    # starting y
    if anchor_y is not None:
        y0 = anchor_y + pad
    else:
        # fallback near bottom of page
        y0 = max(page.rect.height - bottom_margin - block_h, 120.0)

    # keep on page
    if y0 + block_h > page.rect.height - bottom_margin:
        y0 = max(page.rect.height - bottom_margin - block_h, 120.0)

    # subtle white band for readability
    band = fitz.Rect(36, y0 - 8, page.rect.width - 36, y0 + block_h)
    page.draw_rect(band, color=(1, 1, 1), fill=(1, 1, 1))

    # --- helper: draw "fake bold" red text (portable)
    def draw_red_bold(x: float, y: float, text: str, size: int):
        # draw 3 times with tiny offsets to thicken
        for dx, dy in ((0, 0), (0.25, 0), (0, 0.25)):
            page.insert_text((x + dx, y + dy), text,
                             fontsize=size, fontname="helv",  # core font
                             fill=(1, 0, 0))                  # red

    # header + rows
    y = y0
    draw_red_bold(left, y, "FINAL RESULTS", header_size)
    y += line + 6

    for r in sorted(payload.results, key=lambda r: r.diagnosis.lower()):
        draw_red_bold(left, y, f"{r.diagnosis}: {r.final}", row_size)
        y += line

    out_bytes = doc.tobytes()
    doc.close()

    # optional local copy (backend also saves returned bytes)
    (RESULTS_DIR / f"DigLab-{lab}-results.pdf").write_bytes(out_bytes)

    return Response(content=out_bytes, media_type="application/pdf")

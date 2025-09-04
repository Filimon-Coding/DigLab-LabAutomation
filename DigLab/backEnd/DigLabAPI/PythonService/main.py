# backEnd/DigLabAPI/PythonService/main.py
from fastapi import FastAPI, HTTPException, Response, Query
from fastapi.responses import FileResponse
from pydantic import BaseModel, Field
from typing import List, Optional
from pathlib import Path
from datetime import datetime
import uuid, csv, qrcode

from pdf_form import generate_lab_form_pdf

BASE_DIR = Path(__file__).parent
CSV_PATH = BASE_DIR / "samples.csv"
BARCODES_DIR = BASE_DIR / "barcodes"
FORMS_DIR = BASE_DIR / "forms"
BARCODES_DIR.mkdir(parents=True, exist_ok=True)
FORMS_DIR.mkdir(parents=True, exist_ok=True)

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
    personnummer: Optional[str] = None     # <— NEW
    labnummer: Optional[str] = None
    qr_data: Optional[str] = None          # <— NEW: explicit QR payload

app = FastAPI(title="DigLab PyService", version="1.1.0")

def ensure_csv():
    if not CSV_PATH.exists():
        with CSV_PATH.open("w", newline="", encoding="utf-8") as f:
            csv.writer(f).writerow(["labnummer","personnummer","date","time","created_at"])

def make_labnummer(date_iso: str) -> str:
    ymd = date_iso.replace("-", "")
    return f"LAB-{ymd}-{uuid.uuid4().hex[:8].upper()}"

def append_csv(labnummer: str, personnummer: str, date_iso: str, time_hm: str):
    ensure_csv()
    with CSV_PATH.open("a", newline="", encoding="utf-8") as f:
        csv.writer(f).writerow([labnummer, personnummer, date_iso, time_hm, datetime.now().isoformat(timespec="seconds")])

def read_rows():
    if not CSV_PATH.exists():
        return []
    with CSV_PATH.open("r", newline="", encoding="utf-8") as f:
        return list(csv.DictReader(f))

def find_by_labnummer(lab: str):
    for r in read_rows():
        if r.get("labnummer","").strip().upper() == lab.strip().upper():
            return r
    return None

def find_by_personnummer(pp: str):
    for r in read_rows():
        if r.get("personnummer","").strip() == pp.strip():
            return r
    return None

def make_qr_png(data: str) -> Path:
    out = BARCODES_DIR / f"{data}.png"
    qrcode.make(data).save(out)
    return out

@app.get("/")
def root():
    return {"service": "DigLab PyService", "endpoints": ["/health","/register","/barcode","/lookup","/generate-form"]}

@app.get("/health")
def health():
    return {"status": "ok"}

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
    """
    Generate QR for either personnummer OR labnummer (testing-friendly).
    Priority: personnummer -> labnummer -> generate labnummer using date.
    """
    if req.personnummer:
        payload = req.personnummer
    elif req.labnummer:
        payload = req.labnummer
    else:
        if not req.date:
            raise HTTPException(status_code=400, detail="Provide personnummer or labnummer or date")
        # generate a labnummer if nothing else given
        payload = make_labnummer(req.date)

    png_path = make_qr_png(payload)
    return {"qr_data": payload, "png": str(png_path)}

@app.get("/lookup")
def lookup(labnummer: Optional[str] = Query(None), personnummer: Optional[str] = Query(None)):
    """
    Flexible lookup:
    - /lookup?labnummer=LAB-...
    - /lookup?personnummer=12345678901
    """
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
    # validate date/time
    try:
        datetime.strptime(req.date, "%Y-%m-%d")
        datetime.strptime(req.time, "%H:%M")
    except ValueError:
        raise HTTPException(status_code=400, detail="Invalid date/time format")

    # choose labnummer (or generate)
    labnummer = req.labnummer or make_labnummer(req.date)

    # choose QR payload:
    # - if qr_data provided, use it (e.g., personnummer for testing)
    # - else default to labnummer
    qr_payload = (req.qr_data or labnummer).strip()
    qr_png = make_qr_png(qr_payload)

    # build PDF
    pdf_path = FORMS_DIR / f"{labnummer}.pdf"
    generate_lab_form_pdf(
        out_pdf=pdf_path,
        labnummer=labnummer,
        name=req.name,
        date=req.date,
        time=req.time,
        diagnoses=req.diagnoses,
        qr_png=qr_png,
        personnummer=req.personnummer,  # show on the form if provided
    )

    return FileResponse(pdf_path, media_type="application/pdf", filename=pdf_path.name)

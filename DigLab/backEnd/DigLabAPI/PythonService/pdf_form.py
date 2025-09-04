# backEnd/DigLabAPI/PythonService/pdf_form.py
from pathlib import Path
from typing import List
from reportlab.lib.pagesizes import A4
from reportlab.lib.styles import getSampleStyleSheet
from reportlab.platypus import (
    SimpleDocTemplate, Paragraph, Spacer, Table, TableStyle, Image
)
from reportlab.lib import colors
from reportlab.lib.units import mm

ALL_DIAGNOSES = ["Dengue", "Malaria", "TBE", "Hantavirus – Puumalavirus (PuV)"]

def generate_lab_form_pdf(
    out_pdf: Path,
    *,
    labnummer: str,
    name: str,
    date: str,
    time: str,
    diagnoses: List[str],
    qr_png: Path,
    personnummer: str | None = None,
    hospital: str = "DigLab",
    qr_label: str | None = None,  # if None, will use labnummer
) -> None:
    """
    Build a printable lab form PDF.
    - QR in top-right (PNG provided)
    - Patient info (incl. optional Personnummer)
    - Diagnoses with columns: Requested / Positive / Negative
    """
    styles = getSampleStyleSheet()
    doc = SimpleDocTemplate(
        str(out_pdf),
        pagesize=A4,
        leftMargin=24, rightMargin=24, topMargin=24, bottomMargin=24
    )
    elements = []

    # ---------- Header with QR ----------
    qr_img = Image(str(qr_png), width=30*mm, height=30*mm)
    header = Table(
        [
            [Paragraph(f"<b>{hospital}</b>", styles["Heading2"]),
             Paragraph("<b>BLODPRØVESKJEMA</b>", styles["Title"]),
             qr_img]
        ],
        colWidths=[70*mm, 80*mm, 40*mm]
    )
    header.setStyle(TableStyle([
        ('VALIGN', (0,0), (-1,-1), 'MIDDLE')
    ]))
    elements.append(header)

    # small line indicating what the QR encodes
    qr_text = qr_label or labnummer
    elements.append(Paragraph(f"<font size=9><b>QR encodes:</b> {qr_text}</font>", styles["Normal"]))
    elements.append(Spacer(1, 8))

    # ---------- Patient info ----------
    info_rows = [
        ["Full Name:", name],
        ["Date:", date],
        ["Time:", time],
        ["Labnummer:", labnummer],
    ]
    if personnummer:
        info_rows.insert(1, ["Personnummer:", personnummer])

    elements.append(Paragraph("<b>Patient Information</b>", styles["Heading3"]))
    info_tab = Table(info_rows, colWidths=[200, 300])
    info_tab.setStyle(TableStyle([
        ('GRID', (0,0), (-1,-1), 0.25, colors.grey),
        ('VALIGN', (0,0), (-1,-1), 'MIDDLE')
    ]))
    elements.append(info_tab)
    elements.append(Spacer(1, 8))

    # ---------- Diagnoses ----------
    elements.append(Paragraph("<b>Requested Analyses & Results</b>", styles["Heading3"]))

    def mark(x: bool) -> str:
        return "[X]" if x else "[ ]"

    sel = set(diagnoses or [])
    diag_rows = [["Diagnosis", "Requested", "Positive", "Negative"]]
    for d in ALL_DIAGNOSES:
        diag_rows.append([d, mark(d in sel), "[ ]", "[ ]"])
    # freeform "Other"
    diag_rows.append(["Other: __________________________", "[ ]", "[ ]", "[ ]"])

    diag_tab = Table(diag_rows, colWidths=[260, 100, 80, 80])
    diag_tab.setStyle(TableStyle([
        ('BACKGROUND', (0,0), (-1,0), colors.lightgrey),
        ('GRID', (0,0), (-1,-1), 1, colors.black),
        ('ALIGN', (1,1), (-1,-1), 'CENTER'),
        ('ALIGN', (0,1), (0,-1), 'LEFT'),
        ('VALIGN', (0,0), (-1,-1), 'MIDDLE'),
    ]))
    elements.append(diag_tab)
    elements.append(Spacer(1, 8))

    # ---------- Signatures ----------
    elements.append(Paragraph("<b>Signatures</b>", styles["Heading3"]))
    sign_tab = Table(
        [
            ["Collected by:", "___________________ (Signature + ID)"],
            ["Received by:", "___________________ (Signature + Timestamp)"]
        ],
        colWidths=[200, 300]
    )
    sign_tab.setStyle(TableStyle([
        ('GRID', (0,0), (-1,-1), 0.25, colors.grey),
        ('VALIGN', (0,0), (-1,-1), 'MIDDLE')
    ]))
    elements.append(sign_tab)

    # ---------- Build ----------
    doc.build(elements)

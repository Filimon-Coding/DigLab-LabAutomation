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

BOX_BLANK = "[    ]"  # 4 spaces, To make easier for human mistakes 
BOX_X     = "[X]"     # 1 space, registert automatic from pdf form
BOX_EMPTY = "[ ]"     # 1 space, automaticly left empty, if no dignoesis

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
    qr_label: str | None = None,
) -> None:
    """
    Build a printable lab form PDF tailored for robust detection:
    - Monospace checkboxes in Requested / Positive / Negative columns.
    - Positive/Negative are always BOX_BLANK so pen marks are needed.
    - Tight padding around tokens so PyMuPDF search_for() gives precise boxes.
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

    # ---------- Instructions ----------
    elements.append(Paragraph(
        "<font size=9>Mark the results clearly inside the boxes. "
        "Requested is pre-marked; Positive/Negative must be marked on paper.</font>",
        styles["Normal"]
    ))
    elements.append(Spacer(1, 4))

    # ---------- Diagnoses ----------
    elements.append(Paragraph("<b>Requested Analyses & Results</b>", styles["Heading3"]))

    sel = set(diagnoses or [])
    diag_rows = [["Diagnosis", "Requested", "Positive", "Negative"]]
    for d in ALL_DIAGNOSES:
        requested_cell = BOX_X if d in sel else BOX_EMPTY
        diag_rows.append([d, requested_cell, BOX_BLANK, BOX_BLANK])

    # Freeform "Other"
    diag_rows.append(["Other: __________________________", BOX_BLANK, BOX_BLANK, BOX_BLANK])

    # Wider first column; narrow checkbox columns so tokens stay compact
    diag_tab = Table(diag_rows, colWidths=[260, 80, 80, 80])

    diag_tab.setStyle(TableStyle([
        # header
        ('BACKGROUND', (0,0), (-1,0), colors.lightgrey),
        ('TEXTCOLOR',  (0,0), (-1,0), colors.black),
        ('ALIGN',      (0,0), (-1,0), 'CENTER'),
        ('FONTNAME',   (0,0), (-1,0), 'Helvetica-Bold'),
        ('FONTSIZE',   (0,0), (-1,0), 11),

        # grid
        ('GRID', (0,0), (-1,-1), 1, colors.black),

        # content alignment
        ('ALIGN',      (0,1), (0,-1), 'LEFT'),
        ('VALIGN',     (0,1), (-1,-1), 'MIDDLE'),

        # --- checkbox columns use monospace + tight padding ---
        ('FONTNAME',   (1,1), (-1,-1), 'Courier'),  # <-- monospace for [    ]
        ('FONTSIZE',   (1,1), (-1,-1), 12),

        # Tighten padding so search_for() box ≈ visible token width/height
        ('LEFTPADDING',  (1,1), (-1,-1), 2),
        ('RIGHTPADDING', (1,1), (-1,-1), 2),
        ('TOPPADDING',   (1,1), (-1,-1), 2),
        ('BOTTOMPADDING',(1,1), (-1,-1), 2),

        # Optional: a touch more vertical room for handwriting
        ('TOPPADDING',   (0,1), (0,-1), 3),
        ('BOTTOMPADDING',(0,1), (0,-1), 3),
    ]))
    elements.append(diag_tab)
    elements.append(Spacer(1, 8))

    # ---------- Signatures ----------
    elements.append(Paragraph("<b>Signatures</b>", styles["Heading3"]))
    sign_tab = Table(
        [
            ["Collected by:", "___________________ (Signature + ID)"],
            ["Received by:",  "___________________ (Signature + Timestamp)"]
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

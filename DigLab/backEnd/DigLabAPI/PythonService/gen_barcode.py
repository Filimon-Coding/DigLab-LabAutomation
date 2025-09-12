#!/usr/bin/env python3
"""
Generate barcode files for lab numbers from samples.csv.

Usage:
  # Generate for one labnummer
  python3 gen_barcode.py LAB-20250902-A6078222F0

  # Generate for all labnumre in samples.csv
  python3 gen_barcode.py --all

Outputs:
  - barcodes/<LAB>.png  (QR)           if 'qrcode' is available
  - barcodes/<LAB>.svg  (Code128)      if 'python-barcode' is available
  - barcodes/<LAB>.png  (Code128 PNG)  if 'python-barcode' + Pillow available
  - barcodes/<LAB>.zpl  (ZPL fallback) if no libs installed
"""
import sys
import csv
from pathlib import Path

# ---- paths ----
CSV_PATH = Path(__file__).with_name("samples.csv")
OUT_DIR  = Path(__file__).with_name("barcodes")
OUT_DIR.mkdir(exist_ok=True)

def make_qr_png(data: str, filepath: Path) -> bool:
    try:
        import qrcode  # type: ignore
        img = qrcode.make(data)
        img.save(filepath)
        return True
    except Exception:
        return False

def make_code128_svg_png(data: str, basepath: Path) -> bool:
    try:
        from barcode import Code128  # type: ignore
        from barcode.writer import ImageWriter, SVGWriter  # type: ignore

        # SVG (crisp for printing)
        svg_path = basepath.with_suffix(".svg")
        with open(svg_path, "wb") as fsvg:
            Code128(data, writer=SVGWriter()).write(fsvg)

        # Try PNG (nice for preview / MS Word etc.)
        ##
        try:
            png_path = basepath.with_suffix(".png")
            with open(png_path, "wb") as fpng:
                Code128(data, writer=ImageWriter()).write(fpng)
        except Exception:
            pass
        return True
    except Exception:
        return False

def zpl_for_code128(data: str) -> str:
    # Simple ZPL: Code128 + human-readable text under
    return f"""^XA
^PW600
^LH20,20
^FO20,20^BCN,120,Y,N,N
^FD{data}^FS
^FO20,160^A0N,30,30^FD{data}^FS
^XZ
""".strip()

def write_zpl(data: str, basepath: Path):
    zpl_path = basepath.with_suffix(".zpl")
    zpl_path.write_text(zpl_for_code128(data), encoding="utf-8")
    return zpl_path

def generate_for(labnummer: str):
    base = OUT_DIR / labnummer
    # 1) try QR PNG
    if make_qr_png(labnummer, base.with_suffix(".png")):
        print(f"✓ QR → {base.with_suffix('.png')}")
        return
    # 2) try Code128 SVG/PNG
    if make_code128_svg_png(labnummer, base):
        print(f"✓ Code128 → {base.with_suffix('.svg')} (and PNG if possible)")
        return
    # 3) fallback ZPL
    zpl_path = write_zpl(labnummer, base)
    print(f"✓ ZPL fallback → {zpl_path} (send to Zebra)")

def read_all_labnumre():
    if not CSV_PATH.exists():
        print("⚠️  samples.csv not found — nothing to generate.")
        return []
    labs = []
    with CSV_PATH.open("r", newline="", encoding="utf-8") as f:
        reader = csv.DictReader(f)
        for row in reader:
            lab = (row.get("labnummer") or "").strip()
            if lab:
                labs.append(lab)
    return labs

def main():
    if len(sys.argv) >= 2 and sys.argv[1] == "--all":
        all_labs = read_all_labnumre()
        if not all_labs:
            print("No lab numbers found in samples.csv.")
            return
        for lab in all_labs:
            generate_for(lab)
        return

    if len(sys.argv) >= 2:
        lab = sys.argv[1].strip()
        if lab:
            generate_for(lab)
            return

    print(__doc__)

if __name__ == "__main__":
    main()

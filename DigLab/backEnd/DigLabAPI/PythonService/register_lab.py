#!/usr/bin/env python3
"""
Registers a blood sample (file-based, no DB).
- Prompts for: personnummer (11), date (YYYY-MM-DD), time (HH:MM)
- Generates labnummer: LAB-YYYYMMDD-XXXXXXXXXX
- Appends: labnummer,personnummer,dato,klokke,created_at to samples.csv
"""
import csv, re, uuid
from datetime import datetime
from pathlib import Path

CSV_PATH = Path(__file__).with_name("samples.csv")
PERSONNR_RE = re.compile(r"^\d{11}$")
DATE_FMT = "%Y-%m-%d"
TIME_FMT = "%H:%M"

def ensure_csv():
    if not CSV_PATH.exists():
        with CSV_PATH.open("w", newline="", encoding="utf-8") as f:
            csv.writer(f).writerow(["labnummer","personnummer","dato","klokke","created_at"])

def prompt_personnummer():
    while True:
        s = input("Personnummer (11 siffer): ").strip()
        if PERSONNR_RE.match(s): return s
        print("  -> Ugyldig. Skriv 11 siffer, f.eks. 01019912345.")

def prompt_date():
    while True:
        s = input("Dato (YYYY-MM-DD): ").strip()
        try:
            datetime.strptime(s, DATE_FMT); return s
        except ValueError:
            print("  -> Ugyldig dato. Eksempel: 2025-09-02.")

def prompt_time():
    while True:
        s = input("Klokke (HH:MM): ").strip()
        try:
            datetime.strptime(s, TIME_FMT); return s
        except ValueError:
            print("  -> Ugyldig klokkeslett. Eksempel: 14:37.")

def gen_labnummer():
    return f"LAB-{datetime.now():%Y%m%d}-{uuid.uuid4().hex[:10].upper()}"

def append_sample(labnummer, personnummer, dato, klokke):
    ensure_csv()
    with CSV_PATH.open("a", newline="", encoding="utf-8") as f:
        csv.writer(f).writerow([labnummer, personnummer, dato, klokke,
                                datetime.now().isoformat(timespec="seconds")])

def main():
    print("=== Registrer blodprøve (fil-basert) ===\n")
    pn = prompt_personnummer()
    d  = prompt_date()
    t  = prompt_time()
    labnr = gen_labnummer()
    append_sample(labnr, pn, d, t)

    print("\n✅ Lagret i samples.csv")
    print(f"  Personnummer : {pn}")
    print(f"  Dato         : {d}")
    print(f"  Klokke       : {t}")
    print(f"  Labnummer    : {labnr}\n")

if __name__ == "__main__":
    main()

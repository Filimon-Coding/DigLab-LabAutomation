#!/usr/bin/env python3
"""
Looks up labnummer -> prints personnummer, date, time from samples.csv.

Usage:
  python3 lookup_lab.py LAB-YYYYMMDD-XXXXXXXXXX
  # or run without args and it will prompt
"""
import sys, csv
from pathlib import Path

CSV_PATH = Path(__file__).with_name("samples.csv")

def lookup(labnummer: str):
    if not CSV_PATH.exists():
        print("⚠️  samples.csv finnes ikke ennå (ingen registreringer).")
        return
    with CSV_PATH.open("r", newline="", encoding="utf-8") as f:
        reader = csv.DictReader(f)
        for row in reader:
            if row.get("labnummer","").strip().upper() == labnummer.strip().upper():
                print("\n✅ Funnet")
                print(f"  Labnummer    : {row['labnummer']}")
                print(f"  Personnummer : {row['personnummer']}")
                print(f"  Dato         : {row['dato']}")
                print(f"  Klokke       : {row['klokke']}")
                print(f"  Registrert   : {row.get('created_at','')}")
                return
    print("⚠️  Fant ikke dette labnummeret i samples.csv.")

def main():
    if len(sys.argv) >= 2:
        labnr = sys.argv[1]
    else:
        labnr = input("Labnummer å slå opp: ").strip()
        if not labnr:
            print("Ingen labnummer oppgitt."); return
    lookup(labnr)

if __name__ == "__main__":
    main()

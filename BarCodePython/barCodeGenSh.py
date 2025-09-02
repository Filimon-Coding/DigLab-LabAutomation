#!/usr/bin/env python3
"""
Nurse input -> validate -> generate lab number -> store to SQLite.

Fields the nurse types:
  - Personnummer: 11 digits (e.g., 01019912345)
  - Dato: YYYY-MM-DD (e.g., 2025-09-02)
  - Klokke: HH:MM 24h (e.g., 14:37)
"""

import re
import uuid
import sqlite3
from datetime import datetime
from pathlib import Path

DB_PATH = Path("lab.db")

PERSONNR_RE = re.compile(r"^\d{11}$")
DATE_FMT = "%Y-%m-%d"
TIME_FMT = "%H:%M"

def ensure_db(path: Path = DB_PATH):
    con = sqlite3.connect(path)
    cur = con.cursor()
    cur.execute("""
        CREATE TABLE IF NOT EXISTS samples (
            labnummer      TEXT PRIMARY KEY,
            personnummer   TEXT NOT NULL,
            collected_at   TEXT NOT NULL,  -- ISO 8601 (YYYY-MM-DDTHH:MM)
            created_at     TEXT NOT NULL   -- ISO 8601 with seconds
        )
    """)
    cur.execute("CREATE INDEX IF NOT EXISTS idx_samples_person ON samples(personnummer)")
    con.commit()
    return con

def prompt_personnummer():
    while True:
        s = input("Personnummer (11 siffer): ").strip()
        if PERSONNR_RE.match(s):
            return s
        print("  -> Ugyldig. Skriv 11 siffer, f.eks. 01019912345.")

def prompt_date():
    while True:
        s = input("Dato (YYYY-MM-DD): ").strip()
        try:
            return datetime.strptime(s, DATE_FMT).date()
        except ValueError:
            print("  -> Ugyldig dato. Eksempel: 2025-09-02.")

def prompt_time():
    while True:
        s = input("Klokke (HH:MM): ").strip()
        try:
            datetime.strptime(s, TIME_FMT)  # just validate
            return s
        except ValueError:
            print("  -> Ugyldig klokkeslett. Eksempel: 14:37.")

def gen_labnummer() -> str:
    # Short, unique PoC ID: LAB-<YYYYMMDD>-<10 hex>
    return f"LAB-{datetime.now():%Y%m%d}-{uuid.uuid4().hex[:10].upper()}"

def save_sample(con, labnummer: str, personnummer: str, collected_iso_min: str):
    cur = con.cursor()
    cur.execute(
        "INSERT INTO samples(labnummer, personnummer, collected_at, created_at) VALUES(?,?,?,?)",
        (labnummer, personnummer, collected_iso_min, datetime.now().isoformat(timespec="seconds"))
    )
    con.commit()

def main():
    print("=== Registrer blodprøve (PoC) ===\n")
    con = ensure_db()

    while True:
        try:
            pn = prompt_personnummer()
            d = prompt_date()
            t = prompt_time()

            # Build combined timestamp in ISO (minutes resolution)
            collected = datetime.strptime(f"{d.isoformat()} {t}", "%Y-%m-%d %H:%M")
            collected_iso_min = collected.isoformat(timespec="minutes")

            labnr = gen_labnummer()
            save_sample(con, labnr, pn, collected_iso_min)

            print("\n✅ Lagret")
            print(f"  Personnummer : {pn}")
            print(f"  Dato         : {d.isoformat()}")
            print(f"  Klokke       : {t}")
            print(f"  Labnummer    : {labnr}\n")

            again = input("Registrere ny prøve? (j/N): ").strip().lower()
            if again != "j":
                print("Ferdig.")
                break

        except KeyboardInterrupt:
            print("\nAvslutter.")
            break
        except Exception as e:
            print(f"⚠️  Feil: {e}\n")

if __name__ == "__main__":
    main()

#!/bin/bash

# === Absolutt base-sti til prosjektet (bruk HOME!) ===
BASE_DIR="$HOME/Documents/MinCodingLinuxV/Prosjekter/5thSemester/DATA3770HelseteknologiProsjekt/Helseteknologi-prosjekt-Collection"

# Sjekk at mappa finnes
if [ ! -d "$BASE_DIR" ]; then
  echo "Fant ikke BASE_DIR: $BASE_DIR"
  exit 1
fi

# Valgfritt: frigjør porter hvis noe henger igjen (kommenter ut hvis du ikke vil)
# fuser -k 5126/tcp 2>/dev/null || true   # .NET
# fuser -k 7001/tcp 2>/dev/null || true   # Python/uvicorn
# fuser -k 5173/tcp 2>/dev/null || true   # Vite

# --- .NET backend ---
gnome-terminal \
  --title="DigLabAPI (.NET)" \
  -- bash -lc "cd \"$BASE_DIR/DigLab/backEnd/DigLabAPI\" \
  && echo '>>> In $(pwd)' \
  && dotnet run \
  ; echo; echo '(.NET) Avsluttet. Trykk ENTER for å lukke...'; read -r"

# --- Python-service ---
gnome-terminal \
  --title="PythonService (uvicorn)" \
  -- bash -lc "cd \"$BASE_DIR/DigLab/backEnd/DigLabAPI/PythonService\" \
  && echo '>>> In $(pwd)' \
  && source .venv/bin/activate \
  && uvicorn main:app --reload --port 7001 \
  ; echo; echo '(Python) Avsluttet. Trykk ENTER for å lukke...'; read -r"

# --- Frontend ---
gnome-terminal \
  --title="frontEnd (Vite)" \
  -- bash -lc "cd \"$BASE_DIR/DigLab/frontEnd\" \
  && echo '>>> In $(pwd)' \
  && npm run dev \
  ; echo; echo '(Frontend) Avsluttet. Trykk ENTER for å lukke...'; read -r"


# --- mySQl DB -- 

# --- MySQL ---
gnome-terminal \
  --title="MySQL (interactive)" \
  -- bash -lc "echo '>>> Kobler til MySQL og velger diglabDB'; \
  MYSQL_PWD=2222 mysql -u root -h 127.0.0.1 diglabDB \
  ; echo; echo '(MySQL) Avsluttet. Trykk ENTER for å lukke...'; read -r"

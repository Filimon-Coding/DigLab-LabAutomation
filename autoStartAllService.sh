#!/bin/bash
# ============================================================
# DigLab - Cross-Platform Startup Script (Windows/macOS/Linux)
# Starts .NET backend, Python FastAPI OCR microservice, and React frontend
# Works from the current project folder — no hardcoded paths
# ============================================================

# Detect base directory
BASE_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

echo "Starting DigLab services from: $BASE_DIR"
echo "------------------------------------------"

# Detect OS platform
case "$OSTYPE" in
  linux*)   PLATFORM="Linux" ;;
  darwin*)  PLATFORM="macOS" ;;
  cygwin*|msys*|win32*) PLATFORM="Windows" ;;
  *)        PLATFORM="Unknown" ;;
esac

echo "Detected platform: $PLATFORM"
echo

# Function to start a service in a new terminal, cross-platform
run_in_new_terminal() {
  local title=$1
  local cmd=$2

  if [[ "$PLATFORM" == "Linux" ]]; then
    gnome-terminal --title="$title" -- bash -lc "$cmd; echo; read -p 'Press ENTER to close...'"

  elif [[ "$PLATFORM" == "macOS" ]]; then
    osascript -e "tell app \"Terminal\" to do script \"$cmd\""

  elif [[ "$PLATFORM" == "Windows" ]]; then
    start cmd /k "$cmd"

  else
    echo "Unsupported platform. Run manually: $cmd"
  fi
}

# --- .NET Backend ---
run_in_new_terminal "DigLab API (.NET)" "
cd \"$BASE_DIR/DigLab/backEnd/DigLabAPI\" &&
dotnet restore &&
dotnet build &&
dotnet run
"

# --- Python FastAPI OCR Microservice ---
run_in_new_terminal "PythonService (FastAPI)" "
cd \"$BASE_DIR/DigLab/backEnd/DigLabAPI/PythonService\" &&
python3 -m venv venv &&
source venv/bin/activate 2>/dev/null || . venv/Scripts/activate &&
pip install -r requirements.txt &&
uvicorn main:app --reload --port 7001
"

# --- Frontend (React + Vite) ---
run_in_new_terminal "FrontEnd (Vite)" "
cd \"$BASE_DIR/DigLab/frontEnd\" &&
# install only if node_modules is missing
if [ ! -d node_modules ]; then
  npm ci || npm install
fi &&
# start vite without interactive prompts
npm run dev -- --host --port 5173 --strictPort
"

# --- Optional MySQL Terminal ---
# Uncomment if needed:
# run_in_new_terminal "MySQL (interactive)" "
# MYSQL_PWD=2222 mysql -u root -h 127.0.0.1 diglabDB
# "

echo "------------------------------------------"
echo "All DigLab services have been started."
echo "Frontend (React):    http://localhost:5173"
echo "Backend (.NET API):  http://localhost:5126"
echo "OCR (FastAPI):       http://localhost:7001"
echo "------------------------------------------"
echo "Keep all terminals open while running the system."

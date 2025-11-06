
---

# **DigLab: A Digital Platform for Laboratory Data and Patient Registration**

**Course:** DATA3770 – Health Technology Project
**Student:** Filimon Nuguse Kaleab
**Supervisors:**

* Associate Professor Tulpesh Patel, Oslo Metropolitan University (OsloMet)
* Consultant Physician Frank Olav Dahler Pettersen, Oslo University Hospital (OUS)
  **Submission Date:** 21 November 2025

**GitHub Project Repository (Source Code):**
[https://github.com/Filimon-Coding/DATA3770-Helseteknologi-prosjekt.git](https://github.com/Filimon-Coding/DATA3770-Helseteknologi-prosjekt.git)

---

## 1. Overview

**DigLab** is a proof-of-concept system that digitalizes manual laboratory workflows.
The prototype demonstrates secure digital registration, OCR-based analysis, and result management for clinical laboratories.

The system integrates:

* **Digital requisition forms** with QR codes.
* **Automatic PDF report generation and analysis.**
* **OCR-based data extraction** using Python and FastAPI.
* **User authentication and authorization** with JWT in ASP.NET Core.
* **Structured data storage** in MySQL with Entity Framework.
* **Frontend** built with React and TypeScript for accessibility and usability.

---

## 2. System Architecture

| Component              | Technology            | Description                                        |
| ---------------------- | --------------------- | -------------------------------------------------- |
| **Frontend**           | React, TypeScript     | Web interface for clinicians and administrators    |
| **Backend API**        | ASP.NET Core (.NET 8) | Handles users, authentication, and lab data        |
| **OCR Service**        | Python, FastAPI       | Analyzes and extracts data from DigLab PDF forms   |
| **Database**           | MySQL                 | Stores users, orders, and results                  |
| **Automation Scripts** | Bash, Python          | Startup, barcode generation, and data registration |

---

## 3. Folder Structure

```
DigLab/
├── backEnd/
│   ├── DigLabAPI/
│   │   ├── Program.cs              # .NET backend entry point
│   │   ├── PythonService/          # OCR microservice (FastAPI)
│   │   │   ├── main.py             # FastAPI server
│   │   │   ├── pdf_form.py         # PDF generation
│   │   │   ├── gen_barcode.py      # Barcode generation
│   │   │   ├── register_lab.py     # Register sample data (CSV)
│   │   │   ├── lookup_lab.py       # Lookup lab number
│   │   │   ├── requirements.txt    # Python dependencies
│   │   │   └── samples.csv         # Synthetic dataset
│   └── ...
├── frontEnd/
│   ├── src/App.tsx                 # React main component
│   ├── src/api.ts                  # API configuration
│   └── ...
├── autoStart.sh                    # Combined startup script
└── README.md
```

---

## 4. Installation and Setup

Open **three separate terminals** in your IDE (one per service). Keep all three running.

### 4.1 Clone the repository

```bash
git clone https://github.com/Filimon-Coding/DATA3770-Helseteknologi-prosjekt.git
cd DATA3770-Helseteknologi-prosjekt
```

### 4.2 Terminal 1 — Backend API (.NET 8)

```bash
cd DigLab/backEnd/DigLabAPI
dotnet restore
dotnet build
dotnet run
```

* Backend URL: **[http://localhost:5126](http://localhost:5126)**
* Swagger: **[http://localhost:5126/swagger](http://localhost:5126/swagger)**

### 4.3 Terminal 2 — OCR Microservice (Python + FastAPI)

```bash
cd DigLab/backEnd/DigLabAPI/PythonService
python3 -m venv venv
source venv/bin/activate
pip install -r requirements.txt
uvicorn main:app --reload --port 7001
```

* OCR service URL: **[http://127.0.0.1:7001](http://127.0.0.1:7001)** (also reachable via **[http://localhost:7001](http://localhost:7001)**)

### 4.4 Terminal 3 — Frontend (React + Vite)

```bash
cd DigLab/frontEnd
npm install
npm run dev
```

* Frontend URL: **[http://localhost:5173](http://localhost:5173)**

### 4.5 Verification

* Visit **[http://localhost:5173](http://localhost:5173)** (frontend).
* Confirm the backend is reachable at **[http://localhost:5126/swagger](http://localhost:5126/swagger)**.
* Confirm the OCR service is running at **[http://localhost:7001](http://localhost:7001)**.

### 4.6 Optional — Combined startup

```bash
./autoStart.sh
```

**Service ↔ Port summary**

| Service            | Command (run in its own terminal)                              | URL                     |
| ------------------ | -------------------------------------------------------------- | ----------------------- |
| Backend API (.NET) | `dotnet run` in `DigLab/backEnd/DigLabAPI`                     | `http://localhost:5126` |
| OCR (FastAPI)      | `uvicorn main:app --reload --port 7001` in `.../PythonService` | `http://localhost:7001` |
| Frontend (Vite)    | `npm run dev` in `DigLab/frontEnd`                             | `http://localhost:5173` |

---


---

## 5. Default Credentials

| Role  | Username | Password |
| ----- | -------- | -------- |
| Admin | admin    | admin123 |

These credentials are stored securely with BCrypt hashing in the database seed file.

---

## 6. Testing Summary

| Test Type             | Tool              | Key Outcome                                               |
| --------------------- | ----------------- | --------------------------------------------------------- |
| API Validation        | Postman / Swagger | Verified authentication, role access, and CRUD operations |
| Security Check        | jwt.io            | Confirmed correct claims, signing, and 3-hour expiry      |
| OCR Evaluation        | FastAPI + PyMuPDF | 30 digital PDFs analyzed with 100% accuracy               |
| Performance Profiling | Firefox Profiler  | 95% GPU usage, <5% CPU load, ~6.9 s page load             |

---

## 7. Known Limitations

* OCR currently supports only generated DigLab PDFs (no handwriting or scanned input).
* Automated unit tests not implemented in this version.
* Not yet integrated with HelseID or FHIR standards.

---

## 8. Future Work

* Add **AI-based handwriting recognition** for handwritten forms.
* Integrate with **HelseID** and **FHIR** for secure interoperability.
* Implement **data visualization dashboards** for infection statistics.
* Deploy as containerized microservices using Docker or Kubernetes.
* Add automated testing pipelines (Pytest, Jest, Cypress).

---

## 9. License

This project was developed as part of the **DATA3770 Health Technology** course at **Oslo Metropolitan University** and is released for educational purposes.

---
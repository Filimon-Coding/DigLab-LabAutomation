Here’s a clean **README.md** draft you can drop in your project root (DigLab/).  
It documents exactly what you did and the setup steps:

```markdown
# DigLab – Setup Guide

This project is split into two parts:

- **backEnd/** → ASP.NET Core 8 Web API (`DigLabAPI`)
- **frontEnd/** → React + TypeScript (Vite)

---

## 📦 Backend (DigLabAPI)

1. Navigate to the backend folder and scaffold the API:

   ```bash
   cd backEnd
   dotnet new webapi -n DigLabAPI
   cd DigLabAPI
   dotnet run
   ```

2. Test in the browser:

   - Swagger: [http://localhost:5126/swagger](http://localhost:5126/swagger)
   - Example endpoint: [http://localhost:5126/weatherforecast](http://localhost:5126/weatherforecast)

---

## 💻 Frontend (React + TypeScript)

1. Navigate to the frontend folder and scaffold with Vite:

   ```bash
   cd frontEnd
   pnpm create vite . -- --template react-ts
   ```

2. When prompted:

   - **Framework:** React  
   - **Variant:** TypeScript  

   This gives a clean **React + TypeScript** project.

3. Install dependencies and run:

   ```bash
   pnpm install
   pnpm dev --port 5173
   ```

4. Check the `src/` folder — it should now contain:

   - `App.tsx`
   - `main.tsx`

5. Open the app in your browser: [http://localhost:5173](http://localhost:5173)

---

## ⚡ Notes

- Later, if you want built-in routing right away, you could scaffold with  
  **React Router v7 + TypeScript**.  
- For now, we’ll keep it simple and add routing manually later.
```
---

# 📝 DigLab – Progress Notes

## 📂 Project Structure

```
DigLab/
 ├── backEnd/
 │    └── DigLabAPI/     # ASP.NET Core 8 Web API
 ├── frontEnd/           # React + TypeScript (Vite)
 └── Docs.MD             # Documentation
```

---

## ✅ Backend (ASP.NET Core 8)

* Created API project:

  ```bash
  cd backEnd
  dotnet new webapi -n DigLabAPI
  cd DigLabAPI
  dotnet run
  ```
* API runs at `http://localhost:5126`
* Swagger UI: `http://localhost:5126/swagger`
* Default test endpoint: `http://localhost:5126/weatherforecast`
* Learned: visiting `/` gives **404** because Web API doesn’t serve a homepage unless you add:

  ```csharp
  app.MapGet("/", () => "DigLab API is running 🚀");
  ```

---

## ✅ Frontend (React + Vite + TypeScript)

* Created frontend with Vite:

  ```bash
  cd frontEnd
  pnpm create vite . -- --template react-ts
  ```

  * **Framework:** React
  * **Variant:** TypeScript
* Installed dependencies:

  ```bash
  pnpm install
  ```
* Start dev server:

  ```bash
  pnpm dev --port 5173
  ```
* Runs at: `http://localhost:5173`
* Initial page: **Vite + React welcome page**

⚠️ Note to self:
Don’t use `pnpm dev run` → the correct command is **`pnpm dev`**

---

## 🔗 Next Steps (planned)

1. Add **CORS policy** in backend so frontend can fetch data.
2. Add a simple `/` endpoint in backend for health check.
3. In React, replace the starter page with a fetch to `/weatherforecast`.
4. Later, replace `WeatherForecast` with real `OrdersController`.

---



Done with scaffolding → real development.



1️⃣ Start with Python service (PyService)

Reason: It’s the “worker” that actually produces the QR + PDF.

Without this, the .NET API can’t return anything meaningful.

You already have the structure, just finish polishing the /generate-form endpoint.

Test it in Swagger UI → make sure you can POST patient info + diagnoses → get back a valid PDF.


👉 Once this step works, you have a working engine.


---

2️⃣ Then connect it to .NET API (DigLabAPI)

Purpose: .NET will be the “orchestrator” between frontend and Python service.

Add an OrdersController or minimal endpoint:

Receives patient form JSON from frontend.

Calls http://localhost:7001/generate-form (PyService).

Returns the PDF file to frontend.


Test with Postman/Swagger: POST to /orders → .NET forwards → Python → returns PDF.


👉 Now your backend pipeline is complete.


---

3️⃣ Finally build the React frontend

Build the registration form (Name, Date, Time, Diagnoses checkboxes).

On submit → call your .NET API /orders.

API responds with PDF → show “Download PDF” button or auto-open.

Test end-to-end: Fill form → PDF with QR + marked diagnoses downloads.


👉 Now you have a full working loop:
React → .NET → Python → PDF → back to browser.


---

4️⃣ (Later)

Add database in .NET to store orders.

Add authentication (nurse login).

Add admin/lab-side UI to view results.

Polish styling (React Router, Tailwind, shadcn/ui).



---

✅ Recommended order:
(1) PyService → (2) .NET API → (3) Frontend

This way, you always have something testable before adding the next layer.


---

👉 Do you want me to make you a step-by-step checklist (like a dev roadmap) so you can tick off each milestone as you build?



Perfect 👍 Here’s a clear dev roadmap checklist you can follow step by step. You can copy this into your Docs.MD so you tick things off as you go.


---

🛠️ DigLab Development Roadmap

✅ Phase 1 – Python Service (PyService)

[R] Move BarCodePython/ into backEnd/PyService/.

[ ] Create main.py with FastAPI entrypoint.

[ ] Add endpoint:

GET /health → returns {"status":"ok"}

POST /generate-form → input = patient info + diagnoses → output = PDF with QR.


[ ] Implement QR generation (top-right of form).

[ ] Implement PDF form with patient info + [X] for selected diagnoses.

[ ] Test in Swagger UI (http://localhost:7001/docs): send a request → confirm a PDF downloads.



---

✅ Phase 2 – .NET Backend (DigLabAPI)

[ ] Enable CORS so frontend can call API from http://localhost:5173.

[ ] Add endpoint /orders in OrdersController:

Input = patient form JSON (name, date, time, diagnoses).

Logic = calls Python service /generate-form.

Output = PDF file (or file download).


[ ] Test with Swagger/Postman: POST /orders → confirm PDF is returned via Python.



---

✅ Phase 3 – React Frontend (frontEnd)

[ ] Create Patient Registration Form with fields:

Personal Name

Date

Klokke (time)

Diagnoses checkboxes (Dengue, Malaria, TBE, Puumala).


[ ] On submit → call backend API /orders.

[ ] Handle API response:

Show “Download PDF” button OR auto-download.


[ ] Test end-to-end: Fill form → see correct PDF with QR + selections.



---

🔄 Phase 4 – Integration Polish

[ ] Add unique Labnummer generation in .NET (instead of frontend).

[ ] Pass Labnummer to Python service.

[ ] Store each order in a database (e.g. SQLite or PostgreSQL).

[ ] Add /orders/{id} endpoint in .NET to fetch past orders.

[ ] Update frontend to list/download past orders.



---

🔐 Phase 5 – (Future Enhancements)

[ ] Authentication (nurse login).

[ ] Admin/lab UI to update results (Positive/Negative).

[ ] Styling with Tailwind + shadcn/ui for better UX.

[ ] Dockerize frontend, backend, and PyService.



---

👉 Recommended workflow each day:

1. Work only on one box at a time.


2. Always test locally (Swagger for APIs, browser for frontend).


3. Only move to next phase when current phase is ✅ working.




---

Would you like me to also prepare a minimal JSON contract (the exact request/response format) for /generate-form and /orders, so you know what fields frontend → backend → Python should exchange?




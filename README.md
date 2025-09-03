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

# Notes : 

--- 



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

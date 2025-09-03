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
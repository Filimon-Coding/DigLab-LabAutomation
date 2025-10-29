// src/api.ts

export const API_BASE = import.meta.env.VITE_API_BASE ?? "http://localhost:5100";

// Henter token fra localStorage og bygger korrekt header
export function authHeader(): Record<string, string> {
  const t = localStorage.getItem("token");
  return t ? { Authorization: `Bearer ${t}` } : {};
}

// POST-kall med JSON-body og token
export async function postJson<T>(url: string, body: any): Promise<T> {
  const headers: Record<string, string> = {
    "Content-Type": "application/json",
    ...authHeader(),
  };

  const resp = await fetch(API_BASE + url, {
    method: "POST",
    headers,
    body: JSON.stringify(body),
  });

  if (!resp.ok) throw new Error(await resp.text());
  return resp.json();
}

// GET-kall med token
export async function getJson<T>(url: string): Promise<T> {
  const headers: Record<string, string> = {
    ...authHeader(),
  };

  const resp = await fetch(API_BASE + url, { headers });
  if (!resp.ok) throw new Error(await resp.text());
  return resp.json();
}

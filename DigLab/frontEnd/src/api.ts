// src/api.ts
export const API_BASE = import.meta.env.VITE_API_BASE ?? "http://localhost:5126";

function authHeaders(extra: HeadersInit = {}) {
  const t = localStorage.getItem("token");
  return t
    ? { ...extra, Authorization: `Bearer ${t}` }
    : extra;
}

export async function getJson<T = any>(path: string) {
  const r = await fetch(`${API_BASE}${path}`, {
    headers: authHeaders({ "Accept":"application/json" }),
    cache: "no-store",
  });
  if (!r.ok) throw new Error(await r.text().catch(()=>r.statusText));
  return r.json() as Promise<T>;
}

export async function postJson<T = any>(path: string, body: any) {
  const r = await fetch(`${API_BASE}${path}`, {
    method: "POST",
    headers: authHeaders({ "Content-Type":"application/json", "Accept":"application/json" }),
    body: JSON.stringify(body),
  });
  if (!r.ok) throw new Error(await r.text().catch(()=>r.statusText));
  return r.json() as Promise<T>;
}

export async function postForm<T = any>(path: string, form: FormData) {
  const r = await fetch(`${API_BASE}${path}`, {
    method: "POST",
    headers: authHeaders(), // let browser set multipart boundary
    body: form,
  });
  if (!r.ok) throw new Error(await r.text().catch(()=>r.statusText));
  // caller decides whether to .json() or .blob(); return Response:
  return (r as unknown) as T;
}

export async function postForBlob(path: string, body: any) {
  const r = await fetch(`${API_BASE}${path}`, {
    method: "POST",
    headers: authHeaders({ "Content-Type":"application/json" }),
    body: JSON.stringify(body),
  });
  if (!r.ok) throw new Error(await r.text().catch(()=>r.statusText));
  return r.blob();
}

export async function getBlob(path: string) {
  const r = await fetch(`${API_BASE}${path}`, {
    headers: authHeaders(),
    cache: "no-store",
  });
  if (!r.ok) throw new Error(await r.text().catch(()=>r.statusText));
  return r.blob();
}

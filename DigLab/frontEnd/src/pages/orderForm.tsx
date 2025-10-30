import { getJson, postForBlob } from "../api";

import { useState } from "react";


const DIAGNOSES = ["Dengue", "Malaria", "TBE", "Hantavirus – Puumalavirus (PuV)"] as const;
type Diagnosis = typeof DIAGNOSES[number];


export default function OrderForm(){
  const [pnr, setPnr] = useState("");
  const [firstName, setFirstName] = useState("");
  const [middleName, setMiddleName] = useState("");
  const [lastName, setLastName] = useState("");
  const [address, setAddress] = useState("");
  const [postal, setPostal] = useState("");
  const [city, setCity] = useState("");
  const [date, setDate] = useState(() => new Date().toISOString().slice(0,10));
  const [time, setTime] = useState(() => {
    const d = new Date(); return `${String(d.getHours()).padStart(2,"0")}:${String(d.getMinutes()).padStart(2,"0")}`;
  });
  const [dx, setDx] = useState<Diagnosis[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  function toggleDx(d: Diagnosis){
    setDx(prev => prev.includes(d) ? prev.filter(x => x !== d) : [...prev, d]);
  }
  function clearPerson(){
    setFirstName(""); setMiddleName(""); setLastName(""); setAddress(""); setPostal(""); setCity("");
  }

  // ---------- FETCH PERSON ----------
  async function handleFetch() {
    setError(null);
    clearPerson();
    if (!/^\d{11}$/.test(pnr)) {
      setError("Personnummer må være 11 siffer.");
      return;
    }
    setLoading(true);
    try {
      // Use helper (adds Authorization) — no preflight fetch
      const p = await getJson<any>(`/api/persons/by-pnr/${pnr}`);
      setFirstName(p.firstName ?? "");
      setMiddleName(p.middleName ?? "");
      setLastName(p.lastName ?? "");
      setAddress([p.addressLine1, p.addressLine2].filter(Boolean).join(", "));
      setPostal(p.postalCode ?? "");
      setCity(p.city ?? "");
    } catch (e: any) {
      const msg = typeof e?.message === "string" ? e.message : "";
      if (msg.includes("404")) setError("Fant ikke person.");
      else setError("Nettverksfeil ved oppslag.");
    } finally {
      setLoading(false);
    }
  }

  // ---------- CREATE ORDER + DOWNLOAD PDF ----------
  async function handleSend(e: React.FormEvent) {
    e.preventDefault();
    setError(null);

    if (!/^\d{11}$/.test(pnr)) {
      setError("Personnummer må være 11 siffer.");
      return;
    }
    if (dx.length === 0) {
      setError("Velg minst én diagnose.");
      return;
    }

    setLoading(true);
    try {
      const payload = { personnummer: pnr, diagnoses: dx, date, time };

      // Single POST that returns the PDF as a blob (Authorization added by helper)
      const blob = await postForBlob(`/api/orders`, payload);

      // Download
      const url = URL.createObjectURL(blob);
      const a = document.createElement("a");
      a.href = url;
      a.download = `DigLab-${new Date().toISOString().replace(/[:T]/g, "").slice(0, 14)}.pdf`;
      document.body.appendChild(a);
      a.click();
      a.remove();
      URL.revokeObjectURL(url);
    } catch (e: any) {
      const msg = typeof e?.message === "string" ? e.message : "";
      if (msg.includes("404")) setError("Person finnes ikke i registeret.");
      else setError(`Feil ved lagring/generering: ${msg || "ukjent feil"}`);
    } finally {
      setLoading(false);
    }
  }

  return (
    <main className="page">
      <h1>Ny ordre</h1>

      <section className="card grid" style={{gridTemplateColumns:"1fr auto", alignItems:"end", marginBottom:12}}>
        <div className="field">
          <label className="label">Personnummer</label>
          <input className="input" value={pnr} onChange={e=>setPnr(e.target.value.trim())} placeholder="12345678901" maxLength={11}/>
        </div>
        <button className="button secondary" onClick={handleFetch} disabled={loading}>
          {loading ? "Henter..." : "Hent"}
        </button>
      </section>

      <section className="card">
        <div className="kv">
          <div className="label">Navn</div>
          <div><strong>{[firstName, middleName, lastName].filter(Boolean).join(" ") || "—"}</strong></div>
          <div className="label">Adresse</div>
          <div><strong>{address || "—"}</strong></div>
          <div className="label">Postnr/Sted</div>
          <div><strong>{[postal, city].filter(Boolean).join(" ") || "—"}</strong></div>
        </div>
      </section>

      <form className="card grid" onSubmit={handleSend} style={{marginTop:12}}>
        <div className="grid grid-2">
          <div className="field">
            <label className="label">Dato</label>
            <input className="input" type="date" value={date} onChange={e=>setDate(e.target.value)} />
          </div>
          <div className="field">
            <label className="label">Tid</label>
            <input className="input" type="time" value={time} onChange={e=>setTime(e.target.value)} />
          </div>
        </div>

        <div>
          <div className="label" style={{marginBottom:6}}>Diagnoser</div>
          <div className="grid" style={{gridTemplateColumns:"repeat(auto-fit,minmax(220px,1fr))"}}>
            {DIAGNOSES.map(d => (
              <label key={d} className="card" style={{display:"flex", gap:10, alignItems:"center", padding:10}}>
                <input type="checkbox" className="checkbox" checked={dx.includes(d)} onChange={()=>toggleDx(d)} />
                {d}
              </label>
            ))}
          </div>
        </div>

        {error && <div className="bad">{error}</div>}

        <div className="btn-row">
          <button className="button" type="submit" disabled={loading}>
            {loading ? "Sender..." : "Send og last ned PDF"}
          </button>
        </div>
      </form>
    </main>
  );
}

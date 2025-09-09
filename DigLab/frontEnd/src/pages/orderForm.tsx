import { useState } from "react";

const DIAGNOSES = ["Dengue", "Malaria", "TBE", "Hantavirus – Puumalavirus (PuV)"] as const;
type Diagnosis = typeof DIAGNOSES[number];

// Sett base-URL for API (bruk Vite env om du vil)
const API_BASE = import.meta.env.VITE_API_BASE ?? "http://localhost:5126";

export default function OrderForm() {
  // 1) Input for PNR + henteknapp
  const [pnr, setPnr] = useState("");

  // 2) Felter som blir autofylt etter «Hent»
  const [firstName, setFirstName] = useState("");
  const [middleName, setMiddleName] = useState("");
  const [lastName, setLastName] = useState("");
  const [address, setAddress] = useState("");
  const [postal, setPostal] = useState("");
  const [city, setCity] = useState("");

  // 3) Skjema for ordre
  const [date, setDate] = useState(() => new Date().toISOString().slice(0,10)); // yyyy-mm-dd
  const [time, setTime] = useState(() => {
    const d = new Date();
    return `${String(d.getHours()).padStart(2,"0")}:${String(d.getMinutes()).padStart(2,"0")}`;
  });
  const [dx, setDx] = useState<Diagnosis[]>([]);

  // UI-state
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  function toggleDx(d: Diagnosis) {
    setDx(prev => prev.includes(d) ? prev.filter(x => x !== d) : [...prev, d]);
  }

  function clearPerson() {
    setFirstName(""); setMiddleName(""); setLastName("");
    setAddress(""); setPostal(""); setCity("");
  }

  async function handleFetch() {
    setError(null);
    clearPerson();

    if (!/^\d{11}$/.test(pnr)) {
      setError("Personnummer må være 11 siffer.");
      return;
    }

    setLoading(true);
    try {
      const res = await fetch(`${API_BASE}/api/persons/by-pnr/${pnr}`);
      if (res.status === 404) { setError("Fant ikke person."); return; }
      if (!res.ok) { setError(`Oppslag feilet (${res.status})`); return; }
      const p = await res.json();

      setFirstName(p.firstName ?? "");
      setMiddleName(p.middleName ?? "");
      setLastName(p.lastName ?? "");
      const addr = [p.addressLine1, p.addressLine2].filter(Boolean).join(", ");
      setAddress(addr);
      setPostal(p.postalCode ?? "");
      setCity(p.city ?? "");
    } catch {
      setError("Nettverksfeil ved oppslag.");
    } finally {
      setLoading(false);
    }
  }

  async function handleSend(e: React.FormEvent) {
    e.preventDefault();
    setError(null);

    if (!/^\d{11}$/.test(pnr)) { setError("Personnummer må være 11 siffer."); return; }
    if (dx.length === 0) { setError("Velg minst én diagnose."); return; }

    setLoading(true);
    try {
      // Backend lager navn, labnummer, og PDF
      const payload = {
        personnummer: pnr,
        diagnoses: dx,
        date,      // "YYYY-MM-DD"
        time       // "HH:MM"
      };

      const res = await fetch(`${API_BASE}/api/orders`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(payload)
      });

      if (res.status === 404) { setError("Person finnes ikke i registeret."); return; }
      if (!res.ok) {
        const t = await res.text();
        setError(`Feil ved lagring/generering: ${res.status} ${t}`);
        return;
      }

      // Svar er PDF → last ned
      const blob = await res.blob();
      const url = URL.createObjectURL(blob);
      const a = document.createElement("a");
      a.href = url;
      a.download = `DigLab-${new Date().toISOString().replace(/[:T]/g,"").slice(0,14)}.pdf`;
      document.body.appendChild(a);
      a.click();
      a.remove();
      URL.revokeObjectURL(url);
    } catch {
      setError("Nettverksfeil ved innsending.");
    } finally {
      setLoading(false);
    }
  }

  return (
    <div style={{ maxWidth: 640, margin: "1rem auto", padding: 16 }}>
      <h2>Ny ordre</h2>

      {/* PNR + Hent */}
      <div style={{ display: "flex", gap: 8, alignItems: "end", marginBottom: 12 }}>
        <div style={{ flex: 1 }}>
          <label>Personnummer</label>
          <input value={pnr} onChange={e=>setPnr(e.target.value.trim())}
                 placeholder="12345678901" maxLength={11} />
        </div>
        <button onClick={handleFetch} disabled={loading}>
          {loading ? "Henter..." : "Hent"}
        </button>
      </div>

      {/* Autofylte felter (read-only) */}
      <fieldset style={{ marginBottom: 12 }}>
        <legend>Person</legend>
        <div>Navn: <strong>{[firstName, middleName, lastName].filter(Boolean).join(" ")}</strong></div>
        <div>Adresse: <strong>{address}</strong></div>
        <div>Postnr/Sted: <strong>{postal} {city}</strong></div>
      </fieldset>

      {/* Ordre-skjema */}
      <form onSubmit={handleSend} style={{ display: "grid", gap: 12 }}>
        <div style={{ display: "flex", gap: 8 }}>
          <label>Dato
            <input type="date" value={date} onChange={e=>setDate(e.target.value)} />
          </label>
          <label>Tid
            <input type="time" value={time} onChange={e=>setTime(e.target.value)} />
          </label>
        </div>

        <fieldset style={{ padding: 12 }}>
          <legend>Diagnoser</legend>
          {DIAGNOSES.map(d => (
            <label key={d} style={{ display: "block" }}>
              <input type="checkbox" checked={dx.includes(d)} onChange={()=>toggleDx(d)} /> {d}
            </label>
          ))}
        </fieldset>

        {error && <div style={{ color: "crimson" }}>{error}</div>}

        <button type="submit" disabled={loading}>
          {loading ? "Sender..." : "Send og last ned PDF"}
        </button>
      </form>
    </div>
  );
}

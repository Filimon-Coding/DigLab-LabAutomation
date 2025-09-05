import { useState } from "react"

const DIAGNOSES = ["Dengue", "Malaria", "TBE", "Hantavirus – Puumalavirus (PuV)"] as const
type Diagnosis = typeof DIAGNOSES[number]

export default function orders() {
  // --- form state ---
  const [name, setName] = useState("")
  const [personnummer, setPersonnummer] = useState("")
  const [date, setDate] = useState("")
  const [time, setTime] = useState("")
  const [dx, setDx] = useState<Diagnosis[]>([])
  const [usePnrForQR, setUsePnrForQR] = useState(true)

  // --- lookup state (for quick tests) ---
  const [lookupLab, setLookupLab] = useState("")
  const [lookupPnr, setLookupPnr] = useState("")
  const [lookupResult, setLookupResult] = useState<any | null>(null)
  const [lookupError, setLookupError] = useState<string | null>(null)

  // endpoints
  const API_BASE = "http://localhost:5126" // .NET API
  const PY_BASE = "http://localhost:7001"  // PyService (for direct lookup only)

  const toggleDx = (d: Diagnosis) =>
    setDx(prev => (prev.includes(d) ? prev.filter(x => x !== d) : [...prev, d]))

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault()

    // quick validations
    if (!name.trim()) return alert("Name is required")
    if (!/^\d{11}$/.test(personnummer)) return alert("Personnummer must be exactly 11 digits")
    if (!date) return alert("Date is required")
    if (!time) return alert("Time is required")

    const body = {
      name,
      date,
      time,
      diagnoses: dx,
      personnummer,                 // show on PDF
      qr_data: usePnrForQR ? personnummer : undefined, // testing: QR encodes PNR
    }

    const resp = await fetch(`${API_BASE}/orders/form`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(body),
    })

    if (!resp.ok) {
      const txt = await resp.text()
      alert("API error: " + txt)
      return
    }

    // Download PDF
    const blob = await resp.blob()
    const url = window.URL.createObjectURL(blob)
    const a = document.createElement("a")
    a.href = url
    a.download = `DigLab-Form-${new Date().toISOString().slice(0,19).replace(/[:T]/g,"")}.pdf`
    document.body.appendChild(a)
    a.click()
    a.remove()
    window.URL.revokeObjectURL(url)
  }

  async function doLookup() {
    setLookupError(null)
    setLookupResult(null)

    const qs =
      lookupLab.trim()
        ? `labnummer=${encodeURIComponent(lookupLab.trim())}`
        : lookupPnr.trim()
          ? `personnummer=${encodeURIComponent(lookupPnr.trim())}`
          : ""

    if (!qs) {
      setLookupError("Enter a labnummer OR a personnummer.")
      return
    }

    try {
      const resp = await fetch(`${PY_BASE}/lookup?${qs}`)
      if (!resp.ok) {
        const t = await resp.text()
        throw new Error(t || resp.statusText)
      }
      const json = await resp.json()
      setLookupResult(json)
    } catch (err: any) {
      setLookupError(err?.message || "Lookup failed")
    }
  }

  return (
    <div style={{ padding: 24, fontFamily: "system-ui", lineHeight: 1.4 }}>
      <h1>DigLab – Generate Lab Form (Demo)</h1>

      {/* --- Generate Form --- */}
      <form onSubmit={handleSubmit} style={{ display: "grid", gap: 12, maxWidth: 420, marginBottom: 32 }}>
        <label>
          Name
          <input required value={name} onChange={e => setName(e.target.value)} />
        </label>

        <label>
          Personnummer (11 digits)
          <input
            required
            inputMode="numeric"
            pattern="\d{11}"
            placeholder="12345678901"
            value={personnummer}
            onChange={e => setPersonnummer(e.target.value)}
          />
        </label>

        <label>
          Date
          <input type="date" required value={date} onChange={e => setDate(e.target.value)} />
        </label>

        <label>
          Time
          <input type="time" required value={time} onChange={e => setTime(e.target.value)} />
        </label>

        <fieldset style={{ border: "1px solid #ccc", padding: 12 }}>
          <legend>Diagnoses</legend>
          {DIAGNOSES.map(d => (
            <label key={d} style={{ display: "block" }}>
              <input type="checkbox" checked={dx.includes(d)} onChange={() => toggleDx(d)} /> {d}
            </label>
          ))}
        </fieldset>

        <label style={{ display: "flex", alignItems: "center", gap: 8 }}>
          <input
            type="checkbox"
            checked={usePnrForQR}
            onChange={(e) => setUsePnrForQR(e.target.checked)}
          />
          Use <b>personnummer</b> as QR payload (testing)
        </label>

        <button type="submit">Generate PDF</button>
      </form>

      {/* --- Lookup Tester --- */}
      <section style={{ maxWidth: 520 }}>
        <h2>Lookup (PyService)</h2>
        <div style={{ display: "grid", gap: 8, gridTemplateColumns: "1fr auto", alignItems: "end" }}>
          <div>
            <label>
              labnummer
              <input placeholder="LAB-YYYYMMDD-XXXXXX" value={lookupLab} onChange={e => setLookupLab(e.target.value)} />
            </label>
          </div>
          <div style={{ textAlign: "center" }}>— OR —</div>
          <div>
            <label>
              personnummer
              <input placeholder="12345678901" value={lookupPnr} onChange={e => setLookupPnr(e.target.value)} />
            </label>
          </div>
          <button onClick={doLookup} style={{ gridColumn: "1 / -1" }}>Lookup</button>
        </div>

        {lookupError && <p style={{ color: "crimson", marginTop: 8 }}>{lookupError}</p>}
        {lookupResult && (
          <pre style={{ background: "#111", color: "#0f0", padding: 12, marginTop: 8, overflowX: "auto" }}>
            {JSON.stringify(lookupResult, null, 2)}
          </pre>
        )}
        <p style={{ color: "#666", marginTop: 8 }}>
          Tip: `/lookup` reads <code>PythonService/samples.csv</code>. Make sure you either call
          <code> /register</code> first, or generate a form while sending <code>personnummer</code> so the
          service appends a row.
        </p>
      </section>
    </div>
  )
}

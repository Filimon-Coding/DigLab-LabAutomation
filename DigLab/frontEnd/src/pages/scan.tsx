// src/pages/scann.tsx
import React, { useMemo, useRef, useState } from "react";

type Mark = "positive" | "negative" | "inconclusive" | "none";
type AnalysisResult = {
  labnummer?: string;
  result?: string;
  confidence?: number;
  found?: {
    name?: string;
    personnummer?: string;
    date?: string;
    time?: string;
    diagnoses?: string[];
  };
  marks?: Record<string, Mark | "positive" | "negative" | "none">;
  [k: string]: any;
};

const API_BASE = import.meta.env.VITE_API_BASE ?? "http://localhost:5126";

export default function Scann() {
  const [file, setFile] = useState<File | null>(null);
  const [objectUrl, setObjectUrl] = useState<string | null>(null);
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [analysis, setAnalysis] = useState<AnalysisResult | null>(null);

  // per-test overrides: diagnosis -> mark ('' means no override)
  const [overrides, setOverrides] = useState<Record<string, Mark | "">>({});
  const [notice, setNotice] = useState<string | null>(null);

  const inputRef = useRef<HTMLInputElement | null>(null);

  function setPickedFile(f: File | null) {
    if (objectUrl) URL.revokeObjectURL(objectUrl);
    setFile(f);
    setAnalysis(null);
    setOverrides({});
    setError(null);
    setNotice(null);
    if (f) setObjectUrl(URL.createObjectURL(f));
    else setObjectUrl(null);
  }

  function onPickFile(e: React.ChangeEvent<HTMLInputElement>) {
    setPickedFile(e.target.files?.[0] ?? null);
  }
  function onDrop(e: React.DragEvent) {
    e.preventDefault();
    setPickedFile(e.dataTransfer.files?.[0] ?? null);
  }
  const onDragOver = (e: React.DragEvent) => e.preventDefault();

  async function analyze() {
    if (!file) { setError("Please select a PDF or image to analyze."); return; }
    try {
      setBusy(true); setError(null); setNotice(null); setAnalysis(null); setOverrides({});
      const form = new FormData();
      form.append("file", file);
      const resp = await fetch(`${API_BASE}/scan/analyze`, { method: "POST", body: form });
      if (!resp.ok) throw new Error(await resp.text() || resp.statusText);
      const json = (await resp.json()) as AnalysisResult;
      setAnalysis(json);

      // initialize overrides for requested diagnoses
      const req = (json.found?.diagnoses ?? []) as string[];
      const initial: Record<string, ""> = {};
      req.forEach(d => (initial[d] = ""));
      setOverrides(initial);
    } catch (err: any) {
      setError(err?.message ?? "Analyze failed");
    } finally {
      setBusy(false);
    }
  }

  // requested diagnoses from analysis
  const requested = useMemo(() => (analysis?.found?.diagnoses ?? []) as string[], [analysis]);

  // map per-test (auto + override -> final)
  const perTest = useMemo(() => {
    if (!analysis) return [];
    const auto = analysis.marks ?? {};
    return requested.map(d => {
      const autoMark = (auto[d] as Mark) ?? "none";
      const override = overrides[d] || "";
      const finalMark = (override || autoMark) as Mark;
      return { diagnosis: d, auto: autoMark, override: (override || null) as Mark | null, final: finalMark };
    });
  }, [analysis, requested, overrides]);

  function buildPayload() {
    return {
      labnummer: analysis?.labnummer ?? null,
      personnummer: analysis?.found?.personnummer ?? null,
      date: analysis?.found?.date ?? null,
      time: analysis?.found?.time ?? null,
      results: perTest,   // [{ diagnosis, auto, override, final }]
      raw: analysis,      // keep full analyzer output
    };
  }

  async function saveToHistory() {
    if (!analysis) { setError("Analyze a file first."); return; }
    try {
      setBusy(true); setError(null); setNotice(null);
      const resp = await fetch(`${API_BASE}/results`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(buildPayload()),
      });
      if (!resp.ok) throw new Error(await resp.text() || resp.statusText);
      setNotice("Saved to history ✅");
    } catch (err: any) {
      setError(err?.message ?? "Save failed");
    } finally {
      setBusy(false);
    }
  }

  // NEW: Send button
  async function sendFinal() {
    if (!analysis) { setError("Analyze a file first."); return; }
    try {
      setBusy(true); setError(null); setNotice(null);
      const resp = await fetch(`${API_BASE}/results/send`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(buildPayload()),
      });
      if (!resp.ok) throw new Error(await resp.text() || resp.statusText);
      setNotice("Results sent ✅");
    } catch (err: any) {
      setError(err?.message ?? "Send failed");
    } finally {
      setBusy(false);
    }
  }

  return (
    <div style={{ padding: 24, fontFamily: "system-ui", lineHeight: 1.4 }}>
      <h1>Scan Lab Result</h1>
      <p style={{ color: "#555", marginTop: -6 }}>
        Upload a PDF (or image). The system detects per-test results. You can override each test before saving or sending.
      </p>

      {/* Upload */}
      <div
        onDrop={onDrop}
        onDragOver={onDragOver}
        onClick={() => inputRef.current?.click()}
        style={{
          border: "2px dashed #cbd5e1",
          padding: 24,
          borderRadius: 14,
          background: "#f8fafc",
          cursor: "pointer",
          maxWidth: 640,
        }}
      >
        <input ref={inputRef} type="file" accept="application/pdf,image/*" style={{ display: "none" }} onChange={onPickFile} />
        {file ? (
          <div>
            <div style={{ display: "flex", alignItems: "center", gap: 12 }}>
              <strong>{file.name}</strong>
              <span style={{ color: "#64748b" }}>{(file.size / 1024).toFixed(1)} KB</span>
              <button type="button" onClick={(e) => { e.stopPropagation(); setPickedFile(null); }} style={{ marginLeft: "auto" }}>
                Remove
              </button>
            </div>
            {objectUrl && (
              <p style={{ marginTop: 8 }}>
                Preview:{" "}
                <a href={objectUrl} target="_blank" rel="noreferrer">
                  Open in new tab
                </a>
              </p>
            )}
          </div>
        ) : (
          <div style={{ textAlign: "center", color: "#475569" }}>
            <div style={{ fontSize: 18, marginBottom: 4 }}>Drop a PDF or image here</div>
            <div>or click to browse</div>
          </div>
        )}
      </div>

      {/* Actions */}
      <div style={{ display: "flex", gap: 8, marginTop: 12, flexWrap: "wrap" }}>
        <button onClick={analyze} disabled={!file || busy}>{busy ? "Analyzing..." : "Analyze"}</button>
        <button onClick={saveToHistory} disabled={!analysis || busy} title={!analysis ? "Analyze first" : ""}>Save to history</button>
        <button onClick={sendFinal} disabled={!analysis || busy} title={!analysis ? "Analyze first" : ""}>Send</button>
      </div>

      {error && <p style={{ color: "crimson", marginTop: 12 }}>{error}</p>}
      {notice && <p style={{ color: "#047857", marginTop: 8 }}>{notice}</p>}

      {/* Summary + per-test table */}
      {analysis && (
        <section style={{ marginTop: 16, display: "grid", gap: 12, gridTemplateColumns: "1fr", maxWidth: 900 }}>
          <div style={{ border: "1px solid #e5e7eb", borderRadius: 12, padding: 16, background: "#fff" }}>
            <h2 style={{ marginTop: 0 }}>Detected</h2>
            <div style={{ display: "grid", gap: 6, marginBottom: 12 }}>
              <Row label="Lab number" value={analysis.labnummer ?? "—"} />
              <Row label="Name" value={analysis.found?.name ?? "—"} />
              <Row label="Personnummer" value={analysis.found?.personnummer ?? "—"} />
              <Row label="Date" value={analysis.found?.date ?? "—"} />
              <Row label="Time" value={analysis.found?.time ?? "—"} />
              <Row label="Diagnoses" value={requested.length ? requested.join(", ") : "—"} />
            </div>

            {/* Per-test editing table */}
            <div style={{ overflowX: "auto" }}>
              <table style={{ width: "100%", borderCollapse: "collapse" }}>
                <thead>
                  <tr>
                    <Th align="left">Diagnosis</Th>
                    <Th>Auto</Th>
                    <Th>Override</Th>
                    <Th>Final</Th>
                    <Th>{""}</Th>
                  </tr>
                </thead>
                <tbody>
                  {perTest.map(({ diagnosis, auto, final }) => (
                    <tr key={diagnosis} style={{ borderTop: "1px solid #e5e7eb" }}>
                      <Td align="left">{diagnosis}</Td>
                      <Td style={{ textTransform: "uppercase" }}>{auto}</Td>
                      <Td>
                        <RadioRow
                          name={`ovr-${diagnosis}`}
                          value={overrides[diagnosis] || ""}
                          onChange={(v) => setOverrides(o => ({ ...o, [diagnosis]: v }))}
                        />
                      </Td>
                      <Td style={{ fontWeight: 600, textTransform: "uppercase" }}>
                        {(overrides[diagnosis] || final) as string}
                      </Td>
                      <Td>
                        <button type="button" onClick={() => setOverrides(o => ({ ...o, [diagnosis]: "" }))}>
                          Clear
                        </button>
                      </Td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </div>

          {/* Raw JSON */}
          <div style={{ border: "1px solid #e5e7eb", borderRadius: 12, padding: 12, background: "#0b1020", color: "#a7f3d0", overflowX: "auto" }}>
            <div style={{ marginBottom: 8, color: "#93c5fd" }}>Raw response</div>
            <pre style={{ margin: 0 }}>{JSON.stringify(analysis, null, 2)}</pre>
          </div>
        </section>
      )}
    </div>
  );
}

function Row({ label, value }: { label: string; value: React.ReactNode }) {
  return (
    <div style={{ display: "grid", gridTemplateColumns: "160px 1fr", gap: 8 }}>
      <div style={{ color: "#64748b" }}>{label}</div>
      <div>{value}</div>
    </div>
  );
}

function Th({ children, align = "center" }: { children: React.ReactNode; align?: "left" | "center" | "right" }) {
  return <th style={{ textAlign: align, padding: "8px 6px", fontSize: 14, color: "#334155" }}>{children}</th>;
}
function Td({ children, align = "center", style = {} as React.CSSProperties }: { children: React.ReactNode; align?: "left" | "center" | "right"; style?: React.CSSProperties }) {
  return <td style={{ textAlign: align, padding: "8px 6px", fontSize: 14, ...style }}>{children}</td>;
}

function RadioRow({
  name,
  value,
  onChange,
}: {
  name: string;
  value: "" | Mark;
  onChange: (v: "" | Mark) => void;
}) {
  const opts: ("" | Mark)[] = ["positive", "negative", "inconclusive"];
  return (
    <div style={{ display: "flex", gap: 14, alignItems: "center", flexWrap: "wrap" }}>
      {opts.map((opt) => (
        <label key={opt} style={{ display: "flex", gap: 6, alignItems: "center" }}>
          <input type="radio" name={name} value={opt} checked={value === opt} onChange={() => onChange(opt)} />
          {opt.toUpperCase()}
        </label>
      ))}
    </div>
  );
}

import React, { useMemo, useRef, useState } from "react";
import { postForm, postJson } from "../api"; // uses Authorization: Bearer <token>

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
  marks?: Record<string, Mark>;
  [k: string]: any;
};

export default function Scan() {
  const [file, setFile] = useState<File | null>(null);
  const [objectUrl, setObjectUrl] = useState<string | null>(null);
  const [busy, setBusy] = useState(false);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [analysis, setAnalysis] = useState<AnalysisResult | null>(null);
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
    setObjectUrl(f ? URL.createObjectURL(f) : null);
  }
  const onPickFile = (e: React.ChangeEvent<HTMLInputElement>) =>
    setPickedFile(e.target.files?.[0] ?? null);

  async function analyze() {
    if (!file) {
      setError("Please select a PDF or image to analyze.");
      return;
    }
    try {
      setBusy(true);
      setError(null);
      setNotice(null);
      setAnalysis(null);
      setOverrides({});

      const form = new FormData();
      form.append("file", file); // (fixed) removed duplicate append

      // postForm attaches Authorization automatically
      const resp: Response = await postForm(`/scan/analyze`, form);
      if (!resp.ok) throw new Error((await resp.text()) || resp.statusText);

      const json = await resp.json();
      const payload: AnalysisResult = json?.analyzer ?? json;
      setAnalysis(payload);

      if (json?.saved?.dir) setNotice(`Saved file to ${json.saved.dir}`);

      const req = (payload.found?.diagnoses ?? []) as string[];
      const initial: Record<string, ""> = {};
      req.forEach((d) => (initial[d] = ""));
      setOverrides(initial);
    } catch (e: any) {
      setError(e?.message || "Analyze failed");
    } finally {
      setBusy(false);
    }
  }

  const requested = useMemo(
    () => ((analysis?.found?.diagnoses ?? []) as string[]),
    [analysis]
  );

  const rows = useMemo(() => {
    if (!analysis) return [];
    const auto = analysis.marks ?? {};
    return requested.map((d) => {
      const autoMark = (auto[d] as Mark) ?? "none";
      const ov = overrides[d] || "";
      const final = (ov || autoMark) as Mark;
      return { d, auto: autoMark, ov: (ov || null) as Mark | null, final };
    });
  }, [analysis, requested, overrides]);

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
      <div style={{ display: "flex", gap: 12, flexWrap: "wrap" }}>
        {opts.map((opt) => (
          <label key={opt} style={{ display: "flex", gap: 6, alignItems: "center" }}>
            <input
              type="radio"
              name={name}
              value={opt}
              checked={value === opt}
              onChange={() => onChange(opt)}
            />
            {opt.toUpperCase()}
          </label>
        ))}
      </div>
    );
  }

  function markToUpper(m: Mark | "") {
    return m ? m.toUpperCase() : "NONE";
  }

  async function saveToHistory() {
    if (!analysis) return;
    const lab = analysis.labnummer;
    if (!lab) {
      setError("Missing lab number in analysis result.");
      return;
    }
    setSaving(true);
    setError(null);
    setNotice(null);
    try {
      const payload = {
        labNumber: lab,
        requested,
        results: rows.map((r) => ({
          diagnosis: r.d,
          auto: markToUpper(r.auto),
          final: markToUpper(r.final),
          overridden: !!overrides[r.d],
        })),
        meta: {
          personnummer: analysis.found?.personnummer ?? null,
          name: analysis.found?.name ?? null,
          date: analysis.found?.date ?? null,
          time: analysis.found?.time ?? null,
          result: analysis.result ?? null,
          confidence: analysis.confidence ?? null,
        },
      };

      // (fixed) use postJson so Authorization is included
    await postJson(`/api/orders/${encodeURIComponent(lab)}/finalize`, payload);


      setNotice("Saved to history.");
    } catch (e: any) {
      setError(e?.message || "Failed to save.");
    } finally {
      setSaving(false);
    }
  }

  return (
    <main className="page">
      <h1>Scan Lab Result</h1>
      <p className="lead">
        Upload a PDF result. The system reads requested tests and detects marks. You can override
        before saving.
      </p>

      {/* Upload */}
      <div className="card" style={{ padding: 0, overflow: "hidden", marginBottom: 12 }}>
        <div
          style={{
            padding: 24,
            borderBottom: "1px solid var(--border)",
            background: "linear-gradient(180deg, rgba(255,255,255,.8), rgba(255,255,255,.6))",
          }}
        >
          <div style={{ display: "flex", gap: 12, alignItems: "center", flexWrap: "wrap" }}>
            <input
              ref={inputRef}
              type="file"
              accept="application/pdf,image/*"
              onChange={onPickFile}
              className="input"
              style={{ maxWidth: 360 }}
            />
            <button className="quiet" onClick={analyze} disabled={!file || busy}>
              {busy ? "Analyzing..." : "Analyze"}
            </button>
            {objectUrl && (
              <a className="pill" href={objectUrl} target="_blank" rel="noreferrer">
                Preview
              </a>
            )}
          </div>
        </div>

        <div style={{ padding: 16 }}>
          {!file && <div style={{ color: "var(--muted)" }}>No file selected.</div>}
          {file && (
            <div style={{ display: "flex", gap: 14, alignItems: "center" }}>
              <strong>{file.name}</strong>
              <span style={{ color: "var(--muted)" }}>{(file.size / 1024).toFixed(1)} KB</span>
              <button className="quiet" onClick={() => setPickedFile(null)} style={{ marginLeft: "auto" }}>
                Remove
              </button>
            </div>
          )}
        </div>
      </div>

      {error && <div className="bad" style={{ marginBottom: 12 }}>{error}</div>}
      {notice && (
        <div className="good" style={{ marginBottom: 12 }}>
          {notice} &nbsp;
          <a href="/history" className="pill">Go to History</a>
        </div>
      )}

      {/* Results */}
      {analysis && (
        <section className="grid">
          <div className="card">
            <h2 style={{ marginTop: 0 }}>Detected</h2>
            <div className="kv" style={{ marginBottom: 12 }}>
              <div className="label">Lab number</div>
              <div>{analysis.labnummer ?? "—"}</div>
              <div className="label">Name</div>
              <div>{analysis.found?.name ?? "—"}</div>
              <div className="label">Personnummer</div>
              <div>{analysis.found?.personnummer ?? "—"}</div>
              <div className="label">Date</div>
              <div>{analysis.found?.date ?? "—"}</div>
              <div className="label">Time</div>
              <div>{analysis.found?.time ?? "—"}</div>
              <div className="label">Requested</div>
              <div>{requested.length ? requested.join(", ") : "—"}</div>
            </div>

            <div style={{ overflowX: "auto" }}>
              <table className="table">
                <thead>
                  <tr>
                    <th>Diagnosis</th>
                    <th>Auto</th>
                    <th>Override</th>
                    <th>Final</th>
                    <th></th>
                  </tr>
                </thead>
                <tbody>
                  {rows.map(({ d, auto, final }) => (
                    <tr key={d}>
                      <td>{d}</td>
                      <td style={{ textTransform: "uppercase" }}>{auto}</td>
                      <td>
                        <RadioRow
                          name={`ov-${d}`}
                          value={overrides[d] || ""}
                          onChange={(v) => setOverrides((o) => ({ ...o, [d]: v }))}
                        />
                      </td>
                      <td style={{ fontWeight: 700, textTransform: "uppercase" }}>
                        {(overrides[d] || final) as string}
                      </td>
                      <td>
                        <button
                          className="quiet"
                          onClick={() => setOverrides((o) => ({ ...o, [d]: "" }))}
                        >
                          Clear
                        </button>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>

            {/* Save actions */}
            <div className="btn-row" style={{ marginTop: 12 }}>
              <button onClick={saveToHistory} disabled={saving}>
                {saving ? "Saving…" : "Save to History"}
              </button>
              <button
                className="outline"
                onClick={() => setOverrides((prev) =>
                  Object.fromEntries(Object.keys(prev).map((k) => [k, ""]))
                )}
                disabled={saving}
              >
                Clear all overrides
              </button>
            </div>
          </div>

          <div className="card" style={{ background: "#0b1020", color: "#a7f3d0" }}>
            <div style={{ color: "#93c5fd", marginBottom: 8 }}>Raw response</div>
            <pre style={{ margin: 0, overflowX: "auto" }}>
              {JSON.stringify(analysis, null, 2)}
            </pre>
          </div>
        </section>
      )}
    </main>
  );
}

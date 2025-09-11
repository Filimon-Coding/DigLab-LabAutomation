import { useEffect, useMemo, useState } from "react";

type OrderView = {
  labNumber: string;
  name: string;
  personnummer?: string;
  date: string;
  time: string;
  diagnoses: string[];
  createdAtUtc: string;
};

type ResultRow = {
  diagnosis: string;
  auto?: "POSITIVE" | "NEGATIVE" | "INCONCLUSIVE" | "NONE" | null;
  final?: "POSITIVE" | "NEGATIVE" | "INCONCLUSIVE" | "NONE" | null;
  overridden?: boolean;
};

type OrderDetails = {
  labNumber: string;
  requested: string[];
  results: ResultRow[];
  overriddenAny?: boolean;
  pdfUrl?: string; // server decides (results first, then forms)
};

const API_BASE = import.meta.env.VITE_API_BASE ?? "http://localhost:5126";

/* ---------- Download helper (explicit only) ---------- */
async function downloadResultsPdf(lab: string) {
  const url = `${API_BASE}/api/orders/${encodeURIComponent(lab)}/pdf?prefer=results&ts=${Date.now()}`;
  const res = await fetch(url, { cache: "no-store" });
  if (!res.ok) {
    const t = await res.text().catch(() => "");
    alert(`No analyzed file found yet (${res.status}). ${t}`);
    return;
  }
  const blob = await res.blob();
  const a = document.createElement("a");
  a.href = URL.createObjectURL(blob);
  a.download = `DigLab-${lab}-results.pdf`;
  document.body.appendChild(a);
  a.click();
  a.remove();
  URL.revokeObjectURL(a.href);
}

/* ---------- Inline PDF modal (uses blob URL) ---------- */
function PdfModal({
  blobUrl,
  title,
  onClose,
  loading,
  error,
}: {
  blobUrl: string | null;
  title?: string;
  onClose: () => void;
  loading: boolean;
  error: string | null;
}) {
  useEffect(() => {
    const prev = document.body.style.overflow;
    document.body.style.overflow = "hidden";
    const onKey = (e: KeyboardEvent) => { if (e.key === "Escape") onClose(); };
    window.addEventListener("keydown", onKey);
    return () => { document.body.style.overflow = prev; window.removeEventListener("keydown", onKey); };
  }, [onClose]);

  return (
    <div
      role="dialog"
      aria-modal="true"
      aria-label={title || "PDF preview"}
      onClick={onClose}
      style={{
        position: "fixed", inset: 0, background: "rgba(0,0,0,.55)",
        display: "flex", alignItems: "center", justifyContent: "center", zIndex: 1000,
        padding: "4vw"
      }}
    >
      <div
        onClick={(e)=>e.stopPropagation()}
        style={{
          width: "min(100%, 92vw)",
          height: "min(92vh, 100%)",
          maxWidth: "1100px",
          background: "var(--bg1)",
          border: "1px solid var(--border)",
          borderRadius: 12,
          boxShadow: "0 20px 40px rgba(16,24,40,.2)",
          display: "flex",
          flexDirection: "column",
          overflow: "hidden"
        }}
      >
        <div style={{
          display:"flex", alignItems:"center", justifyContent:"space-between",
          gap:12, padding:"10px 12px", borderBottom:"1px solid var(--border)"
        }}>
          <div style={{fontWeight:700}}>{title || "PDF"}</div>
          <div className="btn-row">
            <button className="outline" onClick={onClose}>Close</button>
          </div>
        </div>

        <div style={{flex:1, background:"#111", display:"flex", alignItems:"center", justifyContent:"center"}}>
          {loading && <div style={{color:"#ddd", padding:16}}>Loading PDF…</div>}
          {!loading && error && (
            <div className="bad" style={{padding:16}}>Failed to preview PDF: {error}</div>
          )}
          {!loading && !error && blobUrl && (
            <embed src={blobUrl} type="application/pdf" style={{width:"100%", height:"100%", border:0}} />
          )}
        </div>
      </div>
    </div>
  );
}

export default function History(){
  const [items, setItems] = useState<OrderView[]>([]);
  const [loading, setLoading] = useState(true);

  const [selected, setSelected] = useState<string | null>(null);
  const [details, setDetails] = useState<Record<string, OrderDetails>>({});
  const [loadingDetails, setLoadingDetails] = useState<string | null>(null);

  // PDF preview state (blob workflow)
  const [previewOpen, setPreviewOpen] = useState(false);
  const [previewTitle, setPreviewTitle] = useState<string | undefined>(undefined);
  const [previewBlobUrl, setPreviewBlobUrl] = useState<string | null>(null);
  const [previewLoading, setPreviewLoading] = useState(false);
  const [previewError, setPreviewError] = useState<string | null>(null);

  useEffect(() => {
    (async () => {
      try{
        const res = await fetch(`${API_BASE}/api/orders?take=50`);
        if (res.ok) setItems(await res.json());
      } finally { setLoading(false); }
    })();
  }, []);

  async function openRow(lab: string){
    setSelected(curr => curr === lab ? null : lab);
    if (!details[lab]){
      setLoadingDetails(lab);
      try{
        const res = await fetch(`${API_BASE}/api/orders/${encodeURIComponent(lab)}`);
        if (res.ok){
          const d = (await res.json()) as OrderDetails;
          // keep server pdfUrl; we call with prefer=results when fetching for view/download
          if (!d.pdfUrl) d.pdfUrl = `/api/orders/${encodeURIComponent(lab)}/pdf`;
          setDetails(prev => ({...prev, [lab]: d}));
        }
      } finally { setLoadingDetails(null); }
    }
  }

  async function openPreview(lab: string){
    if (previewBlobUrl) { URL.revokeObjectURL(previewBlobUrl); setPreviewBlobUrl(null); }
    setPreviewError(null);
    setPreviewLoading(true);
    setPreviewOpen(true);
    setPreviewTitle(`PDF – ${lab}`);

    try{
      // force analyzed file + cache-bust
      const url = `${API_BASE}/api/orders/${encodeURIComponent(lab)}/pdf?prefer=results&ts=${Date.now()}`;
      const res = await fetch(url, { cache: "no-store" });
      if (!res.ok){
        const t = await res.text().catch(()=> "");
        throw new Error(`No analyzed file yet (${res.status}). ${t}`);
      }
      const blob = await res.blob();
      const objUrl = URL.createObjectURL(new Blob([blob], { type: "application/pdf" }));
      setPreviewBlobUrl(objUrl);
    }catch(e:any){
      setPreviewError(e?.message || "Unknown error");
    }finally{
      setPreviewLoading(false);
    }
  }

  function closePreview(){
    if (previewBlobUrl) URL.revokeObjectURL(previewBlobUrl);
    setPreviewBlobUrl(null);
    setPreviewOpen(false);
  }

  const rows = useMemo(
    () => items.map(x => ({...x, overridden: details[x.labNumber]?.overriddenAny === true})),
    [items, details]
  );

  return (
    <main className="page">
      <h1>History</h1>
      <p className="lead">Recent orders (last 50).</p>

      <div className="card" style={{overflowX:"auto"}}>
        <table className="table">
          <thead>
            <tr>
              <th>Lab #</th>
              <th>Name</th>
              <th>PNR</th>
              <th>Date</th>
              <th>Time</th>
              <th>Diagnoses</th>
              <th>Created (UTC)</th>
              <th></th>
            </tr>
          </thead>
          <tbody>
            {loading && <tr><td colSpan={8}>Loading…</td></tr>}
            {!loading && rows.length === 0 && <tr><td colSpan={8}>No data.</td></tr>}

            {rows.map(x => {
              const isOpen = selected === x.labNumber;
              const det = details[x.labNumber];

              return (
                <>
                  <tr key={x.labNumber} data-clickable="true" onClick={()=>openRow(x.labNumber)}>
                    <td>
                      {x.labNumber}
                      {details[x.labNumber]?.overriddenAny && <span className="pill" style={{marginLeft:8}}>overridden</span>}
                    </td>
                    <td>{x.name}</td>
                    <td>{x.personnummer ?? "—"}</td>
                    <td>{x.date}</td>
                    <td>{x.time}</td>
                    <td>{x.diagnoses?.join(", ")}</td>
                    <td>{new Date(x.createdAtUtc).toLocaleString()}</td>
                    <td>
                      <button className="outline" onClick={(e)=>{ e.stopPropagation(); openRow(x.labNumber); }}>
                        {isOpen ? "Hide" : "Open"}
                      </button>
                    </td>
                  </tr>

                  {isOpen && (
                    <tr key={x.labNumber + ":details"}>
                      <td colSpan={8} style={{ background: "rgba(0,0,0,0.02)" }}>
                        {loadingDetails === x.labNumber && <div>Loading details…</div>}
                        {det && (
                          <div className="details">
                            <div className="card" style={{marginTop:12}}>
                              <div className="card-header">
                                <div>
                                  <div className="card-title" style={{margin:0}}>Details</div>
                                  <div className="card-sub">Requested: {det.requested?.join(", ") || "—"}</div>
                                  {det.overriddenAny && <div className="card-sub">One or more results were overridden.</div>}
                                </div>
                                <div className="btn-row">
                                  <>
                                    <button className="quiet" onClick={() => openPreview(det.labNumber ?? x.labNumber)}>
                                      View analyzed file
                                    </button>
                                    <button onClick={() => downloadResultsPdf(det.labNumber ?? x.labNumber)}>
                                      Download analyzed file
                                    </button>
                                  </>
                                </div>
                              </div>

                              <table className="table compact">
                                <thead>
                                  <tr>
                                    <th>Diagnosis</th>
                                    <th>Auto</th>
                                    <th>Final</th>
                                    <th>Status</th>
                                  </tr>
                                </thead>
                                <tbody>
                                  {det.results?.map(r => (
                                    <tr key={r.diagnosis}>
                                      <td>{r.diagnosis}</td>
                                      <td>{r.auto ?? "—"}</td>
                                      <td>{r.final ?? "—"}</td>
                                      <td>{r.overridden ? "Overridden" : "—"}</td>
                                    </tr>
                                  ))}
                                </tbody>
                              </table>
                            </div>
                          </div>
                        )}
                      </td>
                    </tr>
                  )}
                </>
              );
            })}
          </tbody>
        </table>
      </div>

      {/* Blob-based modal viewer (no browser download) */}
      {previewOpen && (
        <PdfModal
          blobUrl={previewBlobUrl}
          title={previewTitle}
          loading={previewLoading}
          error={previewError}
          onClose={closePreview}
        />
      )}
    </main>
  );
}

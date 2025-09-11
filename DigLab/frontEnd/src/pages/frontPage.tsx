export default function FrontPage(){
  return (
    <main className="page">
      <h1>Welcome to DigLab</h1>
      <p className="lead">A clean medical lab system for clinical orders, scans and history.</p>

      <div className="grid grid-3">
        {/* Create Order */}
        <div className="card tile">
          <div className="card-header">
            <div className="card-title">Create Order</div>
            <div className="btn-row">
              <span className="pill">Fast</span>
              <span className="pill">GDPR aware</span>
            </div>
          </div>
          <p className="card-sub">Search patient by personnummer, choose tests, and generate a requisition PDF.</p>
          <div className="btn-row" style={{marginTop:12}}>
            <a href="/order"><button className="outline">Open</button></a>
          </div>
        </div>

        {/* Scan Result */}
        <div className="card tile">
          <div className="card-header">
            <div className="card-title">Scan Result</div>
            <div className="btn-row">
              <span className="pill">Vision</span>
              <span className="pill">Override</span>
            </div>
          </div>
          <p className="card-sub">Upload a PDF result, auto-detect marks, and finalize outcomes per test.</p>
          <div className="btn-row" style={{marginTop:12}}>
            <a href="/scan"><button className="outline">Open</button></a>
          </div>
        </div>

        {/* History */}
        <div className="card tile">
          <div className="card-header">
            <div className="card-title">History</div>
            <div className="btn-row">
              <span className="pill">Audit</span>
              <span className="pill">Export</span>
            </div>
          </div>
          <p className="card-sub">Browse previous orders and results. Filter by date or personnummer.</p>
          <div className="btn-row" style={{marginTop:12}}>
            <a href="/history"><button className="outline">Open</button></a>
          </div>
        </div>
      </div>
    </main>
  );
}

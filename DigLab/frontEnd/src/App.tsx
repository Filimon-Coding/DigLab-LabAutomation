import { Routes, Route, NavLink, Navigate } from "react-router-dom";
import FrontPage from "./pages/frontPage";
import Order from "./pages/orderForm";
import Scan from "./pages/scan";
//import History from "./pages/history";
          // <Route path="/history" element={<History />} />

export default function App() {
  return (
    <div className="app-shell">
      {/* Top nav */}
      <nav className="app-nav">
        <div className="brand">
          <span className="logo-dot" />
          <span>DigLab</span>
        </div>

        <div className="nav-links">
          <NavLink to="/" end className="navlink">Home</NavLink>
          <NavLink to="/order" className="navlink">Order</NavLink>
          <NavLink to="/scan" className="navlink">Scan</NavLink>
          <NavLink to="/history" className="navlink">History</NavLink>
        </div>
      </nav>

      {/* Pages */}
      <main className="page">
        <Routes>
          <Route path="/" element={<FrontPage />} />
          <Route path="/order" element={<Order />} />
          <Route path="/scan" element={<Scan />} />

          {/* fallback: anything unknown -> home */}
          <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
      </main>
    </div>
  );
}

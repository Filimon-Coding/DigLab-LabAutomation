// src/App.tsx
import { Routes, Route, Link, Navigate } from "react-router-dom";
import FrontPage from "./pages/frontPage";
import Order from "./pages/order";
//import History from "./pages/history";
//import Scann from "./pages/scann";

export default function App() {
  return (
    <div>
      {/* Simple top nav (optional) */}
      <nav style={{ padding: 12, borderBottom: "1px solid #eee" }}>
        <Link to="/" style={{ marginRight: 12 }}>Home</Link>
        <Link to="/order" style={{ marginRight: 12 }}>Order</Link>
        <Link to="/scann" style={{ marginRight: 12 }}>Scann</Link>
        <Link to="/history">History</Link>
      </nav>

      <Routes>
        <Route path="/" element={<FrontPage />} />
        <Route path="/order" element={<Order />} />

        {/* fallback: anything unknown -> home */}
        <Route path="*" element={<Navigate to="/" replace />} />
      </Routes>
    </div>
  );
}

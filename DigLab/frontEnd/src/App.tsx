import { useEffect, useState, type JSX } from "react";
import { NavLink, Route, Routes, Navigate, useLocation } from "react-router-dom";
import FrontPage from "./pages/frontPage";
import OrderForm from "./pages/orderForm";
import Scan from "./pages/scan";
import History from "./pages/history";
import Login from "./pages/Admin/login";
import "./index.css";

/* ---------- små hjelpere ---------- */
function getUser() {
  return localStorage.getItem("user") ?? "";
}
function hasToken() {
  return !!localStorage.getItem("token");
}

/* ---------- logo ---------- */
function Logo() {
  return (
    <svg width="22" height="22" viewBox="0 0 48 48" aria-hidden>
      <defs>
        <linearGradient id="g" x1="0" x2="1" y1="0" y2="1">
          <stop offset="0" stopColor="#2563eb" />
          <stop offset="1" stopColor="#0ea5e9" />
        </linearGradient>
      </defs>
      <rect x="4" y="4" width="40" height="40" rx="10" fill="url(#g)" opacity=".7" />
      <path
        d="M24 10c.8 0 1.5.7 1.5 1.5V20h8.5c.8 0 1.5.7 1.5 1.5v5c0 .8-.7 1.5-1.5 1.5H25.5v8.5c0 .8-.7 1.5-1.5 1.5h-5c-.8 0-1.5-.7-1.5-1.5V28H9.5c-.8 0-1.5-.7-1.5-1.5v-5c0-.8.7-1.5 1.5-1.5H17V11.5c0-.8.7-1.5 1.5-1.5h5Z"
        fill="#2563eb"
      />
    </svg>
  );
}

/* ---------- guard for beskyttede sider ---------- */
function RequireAuth({ children }: { children: JSX.Element }) {
  return hasToken() ? children : <Navigate to="/login" replace />;
}

/* ---------- top-nav ---------- */
function TopNav({
  authed,
  onLogout,
}: {
  authed: boolean;
  onLogout: () => void;
}) {
  const user = getUser();
  return (
    <nav className="nav">
      <div className="nav-inner container">
        <div className="brand">
          <Logo /> DigLab <span className="badge">v1.0</span>
        </div>

        <div style={{ marginLeft: "auto", display: "flex", gap: 6, alignItems: "center" }}>
          <NavLink to="/" end>
            Home
          </NavLink>
          {authed && <NavLink to="/order">Order</NavLink>}
          {authed && <NavLink to="/scan">Scan</NavLink>}
          {authed && <NavLink to="/history">History</NavLink>}
          {!authed ? (
            <NavLink to="/login">Login</NavLink>
          ) : (
            <>
              {user && <span style={{ opacity: 0.8 }}>({user})</span>}
              <button onClick={onLogout}>Logout</button>
            </>
          )}
        </div>
      </div>
    </nav>
  );
}

/* ---------- hoved-app ---------- */
export default function App() {
  const [, setTick] = useState(0); // bare for å trigge re-render ved login/logout
  const loc = useLocation();

  function logout() {
    localStorage.removeItem("token");
    localStorage.removeItem("user");
    setTick((x) => x + 1);
  }

  // Re-render når token endres fra andre tabs/vinduer
  useEffect(() => {
    const h = () => setTick((x) => x + 1);
    window.addEventListener("storage", h);
    return () => window.removeEventListener("storage", h);
  }, []);

  // Re-render når route endres (nyttig etter login som navigerer tilbake)
  useEffect(() => {
    setTick((x) => x + 1);
  }, [loc.pathname]);

  const authed = hasToken();

  return (
    <>
      <TopNav authed={authed} onLogout={logout} />

      <div className="container">
        <Routes>
          <Route path="/" element={<FrontPage />} />
          <Route path="/login" element={<Login />} />

          {/* beskyttede ruter */}
          <Route
            path="/order"
            element={
              <RequireAuth>
                <OrderForm />
              </RequireAuth>
            }
          />
          <Route
            path="/scan"
            element={
              <RequireAuth>
                <Scan />
              </RequireAuth>
            }
          />
          <Route
            path="/history"
            element={
              <RequireAuth>
                <History />
              </RequireAuth>
            }
          />
          {/* fallback */}
          <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
      </div>
    </>
  );
}

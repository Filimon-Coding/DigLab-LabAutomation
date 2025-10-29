import { useEffect, useRef, useState, type JSX } from "react";
import { NavLink, Route, Routes, Navigate, useLocation, useNavigate } from "react-router-dom";
import FrontPage from "./pages/frontPage";
import OrderForm from "./pages/orderForm";
import Scan from "./pages/scan";
import History from "./pages/history";
import Login from "./pages/Admin/login";
import UsersAdmin from "./pages/Admin/users";
import ChangePassword from "./pages/Admin/changePassword";
import "./index.css";

/* ---------- helpers ---------- */
function hasToken() {
  return !!localStorage.getItem("token");
}
function getFirstName() {
  return localStorage.getItem("firstName") ?? "";
}
function getWorkerId() {
  return localStorage.getItem("workerId") ?? "";
}
function cap(s: string){ return s ? s[0].toUpperCase() + s.slice(1).toLowerCase() : ""; }

function safe(s?: string|null){ return (s ?? "").trim(); }
function first3(n: string){ return n ? (n[0].toUpperCase() + n.slice(1).toLowerCase()).slice(0,3) : ""; }

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

/* ---------- auth guard ---------- */
function RequireAuth({ children }: { children: JSX.Element }) {
  return hasToken() ? children : <Navigate to="/login" replace />;
}

/* ---------- top navigation ---------- */
function TopNav({ authed, onLogout }: { authed: boolean; onLogout: () => void }) {
  const [open, setOpen] = useState(false);
  const ref = useRef<HTMLDivElement>(null);
  const nav = useNavigate();

const firstName = safe(localStorage.getItem("firstName"));
const workerId  = safe(localStorage.getItem("workerId")) || safe(localStorage.getItem("user"));
const badge = firstName ? `${first3(firstName)}${workerId}` : workerId || "Account";
  useEffect(() => {
    const handler = (e: MouseEvent) => {
      if (ref.current && !ref.current.contains(e.target as Node)) setOpen(false);
    };
    document.addEventListener("click", handler);
    return () => document.removeEventListener("click", handler);
  }, []);

  return (
    <nav className="nav">
      <div className="nav-inner container">
        <div className="brand">
          <Logo /> DigLab <span className="badge">v1.0</span>
        </div>

        <div style={{ marginLeft: "auto", display: "flex", gap: 6, alignItems: "center" }}>
          <NavLink to="/" end>Home</NavLink>
          {authed && <NavLink to="/order">Order</NavLink>}
          {authed && <NavLink to="/scan">Scan</NavLink>}
          {authed && <NavLink to="/history">History</NavLink>}
          {authed && localStorage.getItem("role") === "admin" && (
            <NavLink to="/admin/users">Users</NavLink>
          )}

          {!authed ? (
            <NavLink to="/login">Login</NavLink>
          ) : (
            <div ref={ref} style={{ position: "relative" }}>
          <button className="btn-ghost" onClick={() => setOpen(!open)} aria-haspopup="menu" aria-expanded={open}>
            {badge}
          </button>


              {open && (
                <div
                  role="menu"
                  style={{
                    position: "absolute",
                    right: 0,
                    top: "110%",
                    background: "var(--bg1)",
                    border: "1px solid var(--border)",
                    borderRadius: 8,
                    boxShadow: "0 8px 20px rgba(0,0,0,.25)",
                    minWidth: 160,
                    zIndex: 10,
                    padding: 6,
                  }}
                >
                  <button
                    className="menu-item"
                    onClick={() => {
                      setOpen(false);
                      nav("/account/profile");
                    }}
                  >
                    Profile
                  </button>
                  <button
                    className="menu-item"
                    onClick={() => {
                      setOpen(false);
                      nav("/account/password");
                    }}
                  >
                    Password
                  </button>
                  <hr style={{ opacity: 0.3, margin: "6px 0" }} />
                  <button className="menu-item" onClick={onLogout}>
                    Logout
                  </button>
                </div>
              )}
            </div>
          )}
        </div>
      </div>
    </nav>
  );
}

/* ---------- main app ---------- */
export default function App() {
  const [, setTick] = useState(0);
  const loc = useLocation();

  function logout() {
    localStorage.removeItem("token");
    localStorage.removeItem("user");
    localStorage.removeItem("role");
    localStorage.removeItem("firstName");
    localStorage.removeItem("workerId");
    setTick((x) => x + 1);
  }

  useEffect(() => {
    const h = () => setTick((x) => x + 1);
    window.addEventListener("storage", h);
    return () => window.removeEventListener("storage", h);
  }, []);

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

          {/* account + admin */}
          <Route path="/account/password" element={<RequireAuth><ChangePassword /></RequireAuth>} />
          <Route path="/account/profile" element={<RequireAuth><div className="card">Profile page (coming soon)</div></RequireAuth>} />
          <Route path="/admin/users" element={<RequireAuth><UsersAdmin /></RequireAuth>} />

          {/* protected */}
          <Route path="/order" element={<RequireAuth><OrderForm /></RequireAuth>} />
          <Route path="/scan" element={<RequireAuth><Scan /></RequireAuth>} />
          <Route path="/history" element={<RequireAuth><History /></RequireAuth>} />

          {/* fallback */}
          <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
      </div>
    </>
  );
}

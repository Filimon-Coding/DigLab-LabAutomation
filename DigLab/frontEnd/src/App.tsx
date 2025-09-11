import { NavLink, Route, Routes } from "react-router-dom";
import FrontPage from "./pages/frontPage";
import OrderForm from "./pages/orderForm";
import Scan from "./pages/scan";
import History from "./pages/history";
import "./index.css";

function Logo(){ return (
  <svg width="22" height="22" viewBox="0 0 48 48" aria-hidden>
    <defs><linearGradient id="g" x1="0" x2="1" y1="0" y2="1">
      <stop offset="0" stopColor="#2563eb"/><stop offset="1" stopColor="#0ea5e9"/>
    </linearGradient></defs>
    <rect x="4" y="4" width="40" height="40" rx="10" fill="url(#g)" opacity=".7" />
    <path d="M24 10c.8 0 1.5.7 1.5 1.5V20h8.5c.8 0 1.5.7 1.5 1.5v5c0 .8-.7 1.5-1.5 1.5H25.5v8.5c0 .8-.7 1.5-1.5 1.5h-5c-.8 0-1.5-.7-1.5-1.5V28H9.5c-.8 0-1.5-.7-1.5-1.5v-5c0-.8.7-1.5 1.5-1.5H17V11.5c0-.8.7-1.5 1.5-1.5h5Z" fill="#2563eb"/>
  </svg>
)}

export default function App(){
  return (
    <>
      <nav className="nav">
        <div className="nav-inner container">
          <div className="brand"><Logo/> DigLab <span className="badge">v1.0</span></div>
          <div style={{marginLeft:"auto", display:"flex", gap:6}}>
            <NavLink to="/" end>Home</NavLink>
            <NavLink to="/order">Order</NavLink>
            <NavLink to="/scan">Scan</NavLink>
            <NavLink to="/history">History</NavLink>
          </div>
        </div>
      </nav>

      <div className="container">
        <Routes>
          <Route path="/" element={<FrontPage/>}/>
          <Route path="/order" element={<OrderForm/>}/>
          <Route path="/scan" element={<Scan/>}/>
          <Route path="/history" element={<History/>}/>
        </Routes>
      </div>
    </>
  );
}

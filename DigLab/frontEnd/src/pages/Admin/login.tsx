
import { useState } from "react";
import { postJson } from "../../api";

export default function Login(){
  const [u,setU]=useState(""); const [p,setP]=useState(""); const [err,setErr]=useState("");

  async function doLogin(e:React.FormEvent){
    e.preventDefault(); setErr("");
    try{
      const r = await postJson<{token:string; username:string; role:string; expiresUtc:string}>("/api/auth/login", { username:u, password:p });
      localStorage.setItem("token", r.token);
      localStorage.setItem("user", r.username);
      window.location.href="/";
    }catch(e:any){ setErr(e.message || "Login failed"); }
  }

  return (
    <div className="card">
      <h2>Login</h2>
      <form onSubmit={doLogin} className="vstack gap-2">
        <input placeholder="Username" value={u} onChange={e=>setU(e.target.value)} required/>
        <input placeholder="Password" type="password" value={p} onChange={e=>setP(e.target.value)} required/>
        <button>Sign in</button>
        {err && <div className="error">{err}</div>}
      </form>
    </div>
  );
}

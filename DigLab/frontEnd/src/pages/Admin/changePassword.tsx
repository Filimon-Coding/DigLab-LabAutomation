

import { useState } from "react";
import { postJson } from "../../api";

export default function ChangePassword(){
  const [cur,setCur] = useState("");
  const [nw,setNew] = useState("");
  const [msg,setMsg] = useState("");

  async function submit(e:React.FormEvent){
    e.preventDefault(); setMsg("");
    await postJson("/api/users/me/password", { currentPassword:cur, newPassword:nw });
    setMsg("Password updated.");
    setCur(""); setNew("");
  }

  return (
    <form onSubmit={submit} className="card vstack gap-2" style={{maxWidth:480}}>
      <h3>Change password</h3>
      <input type="password" placeholder="Current password" value={cur} onChange={e=>setCur(e.target.value)} required/>
      <input type="password" placeholder="New password" value={nw} onChange={e=>setNew(e.target.value)} required/>
      <button>Update</button>
      {msg && <div className="ok">{msg}</div>}
    </form>
  );
}

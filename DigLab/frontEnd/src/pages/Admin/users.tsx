

import { useEffect, useState } from "react";
import { getJson, postJson } from "../../api";

type UserRow = {
  id:number; firstName:string; lastName:string;
  workerId:string; hprNumber:string; profession:string; role:string;
};

export default function UsersAdmin(){
  const [list, setList] = useState<UserRow[]>([]);
  const [msg, setMsg] = useState("");

  // form state
  const [firstName,setFirst] = useState("");
  const [lastName,setLast] = useState("");
  const [workerId,setWorker] = useState("");
  const [hpr,setHpr] = useState("");
  const [profession,setProf] = useState("Nurse");
  const [initialPwd,setPwd] = useState("");

  async function load(){ setList(await getJson<UserRow[]>("/api/users")); }
  useEffect(()=>{ load(); },[]);

  async function create(e:React.FormEvent){
    e.preventDefault(); setMsg("");
    const body = { firstName:lastName?firstName.trim():firstName.trim(),
                   lastName:lastName.trim(), workerId:workerId.trim(),
                   hprNumber:hpr.trim(), profession, initialPassword:initialPwd || null };
    const res = await postJson<{ok:boolean; username:string; tempPassword:string}>("/api/users", body);
    setMsg(`Created ${res.username}. Temp password: ${res.tempPassword}`);
    setFirst(""); setLast(""); setWorker(""); setHpr(""); setProf("Nurse"); setPwd("");
    await load();
  }

  return (
    <div className="vstack gap-4">
      <h2>Employees</h2>

      <form onSubmit={create} className="card vstack gap-2" style={{maxWidth:600}}>
        <h3>Register new employee</h3>
        <input value={firstName} onChange={e=>setFirst(e.target.value)} placeholder="First name" required/>
        <input value={lastName} onChange={e=>setLast(e.target.value)} placeholder="Last name" required/>
        <input value={workerId} onChange={e=>setWorker(e.target.value)} placeholder="Worker ID (login)" required/>
        <input value={hpr} onChange={e=>setHpr(e.target.value)} placeholder="HPR number" required/>
        <select value={profession} onChange={e=>setProf(e.target.value)}>
          <option>Nurse</option><option>Doctor</option><option>Bioengineer</option><option>Other</option>
        </select>
        <input value={initialPwd} onChange={e=>setPwd(e.target.value)} placeholder="Initial password (optional)"/>
        <button>Create</button>
        {msg && <div className="ok">{msg}</div>}
      </form>

      <div className="card">
        <h3>All users</h3>
        <table className="table">
          <thead><tr><th>Name</th><th>WorkerID</th><th>HPR</th><th>Profession</th><th>Role</th></tr></thead>
          <tbody>
            {list.map(u=>(
              <tr key={u.id}>
                <td>{u.lastName}, {u.firstName}</td>
                <td>{u.workerId}</td>
                <td>{u.hprNumber}</td>
                <td>{u.profession}</td>
                <td>{u.role}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}

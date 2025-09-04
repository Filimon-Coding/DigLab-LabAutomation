// src/pages/frontPage.tsx
import React from "react";
import { Link } from "react-router-dom";

/**
 * FrontPage
 * - Simple landing page with three choices.
 * - Uses <Link> so React Router handles navigation without reloads.
 */
export default function FrontPage() {
  return (
    <main
      style={{
        minHeight: "100vh",
        display: "grid",
        placeItems: "center",
        fontFamily: "system-ui",
        background: "linear-gradient(180deg,#f8fafc,#eef2f7)",
        padding: 24,
      }}
    >
      <section style={{ width: "100%", maxWidth: 900 }}>
        <header style={{ textAlign: "center", marginBottom: 24 }}>
          <h1 style={{ margin: 0, fontSize: 28 }}>DigLab</h1>
          <p style={{ marginTop: 6, color: "#555" }}>
            Choose an action to continue
          </p>
        </header>

        {/* Cards container */}
        <div
          style={{
            display: "grid",
            gap: 16,
            gridTemplateColumns: "repeat(auto-fit, minmax(220px, 1fr))",
          }}
        >
          <NavCard
            title="Create Order"
            description="Fill out patient details and generate a PDF form."
            to="/order"
          />
          <NavCard
            title="Scan"
            description="Scan or enter a lab number to retrieve data."
            to="/scann"
          />
          <NavCard
            title="History"
            description="View previously created orders and lookups."
            to="/history"
          />
        </div>
      </section>
    </main>
  );
}

/** Small helper: a clickable card that navigates via <Link> */
function NavCard({
  title,
  description,
  to,
}: {
  title: string;
  description: string;
  to: string;
}) {
  return (
    <Link
      to={to}
      style={{
        display: "block",
        textDecoration: "none",
        color: "inherit",
        borderRadius: 16,
        border: "1px solid #e5e7eb",
        background: "#fff",
        padding: 18,
        boxShadow: "0 1px 3px rgba(0,0,0,.06)",
        transition: "transform .08s ease, box-shadow .08s ease",
      }}
      onMouseEnter={(e) =>
        ((e.currentTarget.style.boxShadow =
          "0 6px 20px rgba(0,0,0,.10)"), (e.currentTarget.style.transform = "translateY(-2px)"))
      }
      onMouseLeave={(e) =>
        ((e.currentTarget.style.boxShadow = "0 1px 3px rgba(0,0,0,.06)"),
        (e.currentTarget.style.transform = "translateY(0)"))
      }
    >
      <h2 style={{ margin: "0 0 8px 0", fontSize: 20 }}>{title}</h2>
      <p style={{ margin: 0, color: "#555" }}>{description}</p>
    </Link>
  );
}

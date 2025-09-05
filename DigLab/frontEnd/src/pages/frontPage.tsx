// src/pages/frontPage.tsx
import React from "react";
import { Link } from "react-router-dom";

/**
 * FrontPage (medical-style)
 * - Calm colors, glass cards, simple icons
 * - Keyboard focus + hover feedback
 * - Works in light & dark (prefers-color-scheme)
 */
export default function FrontPage() {
  return (
    <main style={styles.shell}>
      <div style={styles.backgroundDecor} aria-hidden />

      <section style={styles.container} aria-label="DigLab start page">
        {/* Top brand bar */}
        <header style={styles.header}>
          <div style={styles.brand}>
            <Logo />
            <div>
              <h1 style={styles.title}>DigLab</h1>
              <div style={styles.subtitle}>Clinical orders • Scanning • History</div>
            </div>
          </div>

          <Badge>v1.0</Badge>
        </header>

        {/* Prompt */}
        <p style={styles.lead}>Choose an action to continue</p>

        {/* Cards */}
        <div style={styles.grid}>
          <NavCard
            to="/order"
            title="Create Order"
            description="Register patient and generate a PDF requisition."
            icon={<IconClipboard />}
            accent="var(--accent)"
          />

          <NavCard
            to="/scan"
            title="Scan"
            description="Upload or scan a lab result and verify per-test outcomes."
            icon={<IconScan />}
            accent="var(--teal)"
          />

          <NavCard
            to="/history"
            title="History"
            description="Review previous orders, scans and exports."
            icon={<IconHistory />}
            accent="var(--indigo)"
          />
        </div>

        {/* Footer note */}
        <footer style={styles.footer}>
          <span>Secure • Audit ready • GDPR aware</span>
          <span style={{ opacity: .6 }}>© {new Date().getFullYear()} DigLab</span>
        </footer>
      </section>
    </main>
  );
}

/* ---------- Card ---------- */
function NavCard({
  to,
  title,
  description,
  icon,
  accent,
}: {
  to: string;
  title: string;
  description: string;
  icon: React.ReactNode;
  accent: string;
}) {
  return (
    <Link
      to={to}
      style={{ ...styles.card, borderTopColor: accent }}
      className="navcard"
      aria-label={title}
    >
      <div style={styles.cardIconWrap}>{icon}</div>
      <div style={{ display: "grid", gap: 6 }}>
        <h2 style={styles.cardTitle}>{title}</h2>
        <p style={styles.cardDesc}>{description}</p>
      </div>
      <span style={styles.cardChevron} aria-hidden>›</span>
    </Link>
  );
}

/* ---------- Small UI bits ---------- */
function Badge({ children }: { children: React.ReactNode }) {
  return (
    <span style={styles.badge}>{children}</span>
  );
}

/* ---------- Icons (inline SVG, no deps) ---------- */
function Logo() {
  return (
    <svg width="40" height="40" viewBox="0 0 48 48" aria-hidden>
      <defs>
        <linearGradient id="g" x1="0" x2="1" y1="0" y2="1">
          <stop offset="0" stopColor="var(--accent)" />
          <stop offset="1" stopColor="var(--teal)" />
        </linearGradient>
      </defs>
      <rect x="4" y="4" width="40" height="40" rx="10" fill="url(#g)" opacity=".2" />
      <path
        d="M24 10c.8 0 1.5.7 1.5 1.5V20h8.5c.8 0 1.5.7 1.5 1.5v5c0 .8-.7 1.5-1.5 1.5H25.5v8.5c0 .8-.7 1.5-1.5 1.5h-5c-.8 0-1.5-.7-1.5-1.5V28H9.5c-.8 0-1.5-.7-1.5-1.5v-5c0-.8.7-1.5 1.5-1.5H17V11.5c0-.8.7-1.5 1.5-1.5h5Z"
        fill="var(--accent)"
      />
    </svg>
  );
}

function IconClipboard() {
  return (
    <svg width="26" height="26" viewBox="0 0 24 24" aria-hidden>
      <path fill="currentColor" d="M9 2h6a2 2 0 0 1 2 2h1a2 2 0 0 1 2 2v12.5A3.5 3.5 0 0 1 16.5 22h-9A3.5 3.5 0 0 1 4 18.5V6a2 2 0 0 1 2-2h1a2 2 0 0 1 2-2Zm0 2a1 1 0 0 0-1 1h8a1 1 0 0 0-1-1H9Z"/>
    </svg>
  );
}

function IconScan() {
  return (
    <svg width="26" height="26" viewBox="0 0 24 24" aria-hidden>
      <path fill="currentColor" d="M4 7V5h16v2H4Zm0 12v-2h6v2H4Zm10 0v-2h6v2h-6ZM4 13v-2h16v2H4Z"/>
    </svg>
  );
}

function IconHistory() {
  return (
    <svg width="26" height="26" viewBox="0 0 24 24" aria-hidden>
      <path fill="currentColor" d="M13 3a9 9 0 1 1-9 9H1l3.6-3.6L8 12H5a7 7 0 1 0 7-7Zm-1 4h2v5h4v2h-6V7Z"/>
    </svg>
  );
}

/* ---------- Styles ---------- */
const styles: Record<string, React.CSSProperties> = {
  shell: {
    // CSS variables for easy theming
    // (these work in inline styles by defining on the root element)
    ['--accent' as any]: '#2563eb', // medical blue
    ['--teal' as any]: '#0ea5e9',
    ['--indigo' as any]: '#6366f1',

    minHeight: "100vh",
    fontFamily: "system-ui, -apple-system, Segoe UI, Roboto, Arial, sans-serif",
    background:
      "radial-gradient(1000px 500px at 10% -10%, rgba(37, 99, 235, .10), transparent), radial-gradient(800px 400px at 100% 0%, rgba(14, 165, 233, .10), transparent), linear-gradient(180deg,#f8fafc,#eef2f7)",
    position: "relative",
    padding: 24,
  },
  backgroundDecor: {
    position: "absolute",
    inset: 0,
    backdropFilter: "saturate(115%)",
    pointerEvents: "none",
  },
  container: {
    width: "100%",
    maxWidth: 980,
    margin: "0 auto",
    display: "grid",
    gap: 18,
  },
  header: {
    display: "flex",
    alignItems: "center",
    justifyContent: "space-between",
    padding: 12,
  },
  brand: {
    display: "flex",
    alignItems: "center",
    gap: 12,
  },
  title: {
    margin: 0,
    fontSize: 28,
    letterSpacing: 0.2,
  },
  subtitle: {
    fontSize: 12,
    color: "#5b6576",
  },
  badge: {
    padding: "6px 10px",
    borderRadius: 999,
    border: "1px solid rgba(99,102,241,.2)",
    background: "rgba(99,102,241,.08)",
    color: "#374151",
    fontSize: 12,
  },
  lead: {
    marginTop: 4,
    color: "#475569",
    paddingLeft: 12,
  },
  grid: {
    display: "grid",
    gap: 18,
    gridTemplateColumns: "repeat(auto-fit, minmax(260px, 1fr))",
  },
  card: {
    display: "grid",
    gridTemplateColumns: "auto 1fr auto",
    alignItems: "center",
    gap: 14,
    textDecoration: "none",
    color: "inherit",
    padding: 18,
    borderRadius: 16,
    border: "1px solid rgba(0,0,0,.06)",
    borderTopWidth: 4,
    background: "rgba(255,255,255,.75)",
    boxShadow: "0 6px 18px rgba(0,0,0,.06)",
    transition: "transform .08s ease, box-shadow .12s ease, background .12s ease",
  },
  cardIconWrap: {
    width: 44,
    height: 44,
    borderRadius: 10,
    display: "grid",
    placeItems: "center",
    background: "linear-gradient(180deg, rgba(37,99,235,.10), rgba(37,99,235,.04))",
    color: "var(--accent)",
  },
  cardTitle: {
    margin: 0,
    fontSize: 18,
  },
  cardDesc: {
    margin: 0,
    color: "#5b6576",
    fontSize: 14,
  },
  cardChevron: {
    fontSize: 28,
    lineHeight: 1,
    opacity: .25,
    transform: "translateX(-2px)",
  },
  footer: {
    display: "flex",
    justifyContent: "space-between",
    padding: "12px 6px",
    fontSize: 12,
    color: "#6b7280",
  },
};

/* ----- Tiny interaction: hover/focus styles via JS -----
   (Optional: if you prefer CSS files, move these into App.css.)
*/
if (typeof document !== "undefined") {
  document.addEventListener("mouseover", (e) => {
    const el = (e.target as HTMLElement)?.closest?.(".navcard") as HTMLElement | null;
    if (el) {
      el.style.transform = "translateY(-2px)";
      el.style.boxShadow = "0 14px 30px rgba(37,99,235,.12)";
      el.style.background = "rgba(255,255,255,.9)";
    }
  });
  document.addEventListener("mouseout", (e) => {
    const el = (e.target as HTMLElement)?.closest?.(".navcard") as HTMLElement | null;
    if (el) {
      el.style.transform = "translateY(0)";
      el.style.boxShadow = "0 6px 18px rgba(0,0,0,.06)";
      el.style.background = "rgba(255,255,255,.75)";
    }
  });
}

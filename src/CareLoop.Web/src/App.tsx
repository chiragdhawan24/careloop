import {
  Activity,
  ClipboardCheck,
  Clock3,
  HeartPulse,
  LogOut,
  Phone,
  RefreshCcw,
  Search,
  ShieldCheck,
  TriangleAlert,
} from "lucide-react";

import {
  useCallback,
  useEffect,
  useMemo,
  useState,
} from "react";

import { getCareCases, login } from "./api";

import type {
  CareCaseSummary,
  CareCaseStatus,
  ResultPriority,
} from "./types";

import "./App.css";

const TOKEN_KEY = "careloop_access_token";

function App() {
  const [token, setToken] = useState<string | null>(
    sessionStorage.getItem(TOKEN_KEY),
  );

  if (!token) {
    return (
      <Login
        onAuthenticated={(accessToken) => {
          sessionStorage.setItem(
            TOKEN_KEY,
            accessToken,
          );

          setToken(accessToken);
        }}
      />
    );
  }

  return (
    <Dashboard
      token={token}
      onLogout={() => {
        sessionStorage.removeItem(TOKEN_KEY);
        setToken(null);
      }}
    />
  );
}

function Login({
  onAuthenticated,
}: {
  onAuthenticated: (token: string) => void;
}) {
  const [email, setEmail] = useState(
    "clinician@careloop.local",
  );

  const [password, setPassword] = useState(
    "CareLoopDemo!2026",
  );

  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  async function handleSubmit(
    event: React.FormEvent,
  ) {
    event.preventDefault();

    try {
      setLoading(true);
      setError("");

      const result = await login(
        email,
        password,
      );

      onAuthenticated(result.accessToken);
    } catch {
      setError(
        "Unable to sign in. Check your credentials.",
      );
    } finally {
      setLoading(false);
    }
  }

  return (
    <main className="login-page">
      <section className="login-brand">
        <div className="brand-mark">
          <HeartPulse size={28} />
        </div>

        <span>CareLoop</span>
      </section>

      <section className="login-panel">
        <div className="login-card">
          <div className="security-chip">
            <ShieldCheck size={15} />
            Secure clinical workspace
          </div>

          <h1>Welcome back</h1>

          <p className="login-copy">
            Sign in to manage diagnostic results,
            patient communication, follow-up, and
            escalation workflows.
          </p>

          <form onSubmit={handleSubmit}>
            <label>
              Email
              <input
                type="email"
                value={email}
                onChange={(event) =>
                  setEmail(event.target.value)
                }
                required
              />
            </label>

            <label>
              Password
              <input
                type="password"
                value={password}
                onChange={(event) =>
                  setPassword(event.target.value)
                }
                required
              />
            </label>

            {error && (
              <div className="login-error">
                {error}
              </div>
            )}

            <button
              className="primary-button"
              disabled={loading}
            >
              {loading
                ? "Signing in..."
                : "Sign in to CareLoop"}
            </button>
          </form>
        </div>
      </section>
    </main>
  );
}

function Dashboard({
  token,
  onLogout,
}: {
  token: string;
  onLogout: () => void;
}) {
  const [cases, setCases] = useState<
    CareCaseSummary[]
  >([]);

  const [search, setSearch] = useState("");
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  const loadCases = useCallback(async () => {
    try {
      setLoading(true);
      setError("");

      const result = await getCareCases(token);

      setCases(result);
    } catch (exception) {
      if (
        exception instanceof Error &&
        exception.message === "UNAUTHORIZED"
      ) {
        onLogout();
        return;
      }

      setError(
        "The clinical queue could not be loaded.",
      );
    } finally {
      setLoading(false);
    }
  }, [token, onLogout]);

  useEffect(() => {
    void loadCases();
  }, [loadCases]);

  const metrics = useMemo(() => {
    return {
      active: cases.filter(
        (item) => item.status !== "Resolved",
      ).length,

      awaitingReview: cases.filter(
        (item) =>
          item.status === "AwaitingReview",
      ).length,

      awaitingContact: cases.filter(
        (item) =>
          item.status ===
          "AwaitingPatientContact",
      ).length,

      followUp: cases.filter((item) =>
        [
          "FollowUpRequired",
          "FollowUpInProgress",
        ].includes(item.status),
      ).length,

      critical: cases.filter(
        (item) =>
          item.priority === "Critical" &&
          item.status !== "Resolved",
      ).length,
    };
  }, [cases]);

  const visibleCases = useMemo(() => {
    const query =
      search.trim().toLowerCase();

    if (!query) {
      return cases;
    }

    return cases.filter((item) =>
      [
        item.patientName,
        item.medicalRecordNumber,
        item.testName,
        item.status,
        item.priority ?? "",
      ].some((value) =>
        value.toLowerCase().includes(query),
      ),
    );
  }, [cases, search]);

  return (
    <div className="app-shell">
      <aside className="sidebar">
        <div className="sidebar-brand">
          <div className="brand-mark small">
            <HeartPulse size={22} />
          </div>

          <div>
            <strong>CareLoop</strong>
            <span>Clinical Operations</span>
          </div>
        </div>

        <nav>
          <button className="nav-item active">
            <Activity size={18} />
            Operations
          </button>

          <button className="nav-item">
            <ClipboardCheck size={18} />
            Care cases
          </button>

          <button className="nav-item">
            <TriangleAlert size={18} />
            Escalations
          </button>
        </nav>

        <div className="sidebar-footer">
          <div className="signed-in">
            <span className="avatar">CD</span>

            <div>
              <strong>Clinician</strong>
              <span>Physician</span>
            </div>
          </div>

          <button
            className="logout-button"
            onClick={onLogout}
          >
            <LogOut size={17} />
            Sign out
          </button>
        </div>
      </aside>

      <main className="dashboard">
        <header className="dashboard-header">
          <div>
            <p className="eyebrow">
              Clinical Operations
            </p>

            <h1>
              Diagnostic Follow-up Dashboard
            </h1>

            <p>
              Monitor results and close every
              diagnostic follow-up loop.
            </p>
          </div>

          <button
            className="refresh-button"
            onClick={() => void loadCases()}
          >
            <RefreshCcw size={17} />
            Refresh
          </button>
        </header>

        <section className="metric-grid">
          <Metric
            title="Active cases"
            value={metrics.active}
            icon={<Activity size={20} />}
          />

          <Metric
            title="Awaiting review"
            value={metrics.awaitingReview}
            icon={<ClipboardCheck size={20} />}
          />

          <Metric
            title="Patient contact"
            value={metrics.awaitingContact}
            icon={<Phone size={20} />}
          />

          <Metric
            title="Follow-up"
            value={metrics.followUp}
            icon={<Clock3 size={20} />}
          />

          <Metric
            title="Critical"
            value={metrics.critical}
            icon={<TriangleAlert size={20} />}
            critical={metrics.critical > 0}
          />
        </section>

        <section className="queue-panel">
          <div className="queue-header">
            <div>
              <h2>CareCase work queue</h2>
              <p>
                {visibleCases.length} cases shown
              </p>
            </div>

            <div className="search-box">
              <Search size={17} />

              <input
                placeholder="Search patient, MRN, test..."
                value={search}
                onChange={(event) =>
                  setSearch(event.target.value)
                }
              />
            </div>
          </div>

          {error && (
            <div className="error-banner">
              {error}
            </div>
          )}

          {loading ? (
            <div className="empty-state">
              Loading clinical queue...
            </div>
          ) : (
            <div className="table-container">
              <table>
                <thead>
                  <tr>
                    <th>Patient</th>
                    <th>Diagnostic order</th>
                    <th>Priority</th>
                    <th>Status</th>
                    <th>Opened</th>
                  </tr>
                </thead>

                <tbody>
                  {visibleCases.map((careCase) => (
                    <tr key={careCase.id}>
                      <td>
                        <div className="patient-cell">
                          <strong>
                            {careCase.patientName}
                          </strong>

                          <span>
                            {
                              careCase.medicalRecordNumber
                            }
                          </span>
                        </div>
                      </td>

                      <td>
                        {careCase.testName}
                      </td>

                      <td>
                        <PriorityBadge
                          priority={
                            careCase.priority
                          }
                        />
                      </td>

                      <td>
                        <StatusBadge
                          status={
                            careCase.status
                          }
                        />
                      </td>

                      <td className="date-cell">
                        {new Intl.DateTimeFormat(
                          "en-US",
                          {
                            month: "short",
                            day: "numeric",
                            year: "numeric",
                          },
                        ).format(
                          new Date(
                            careCase.createdAt,
                          ),
                        )}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </section>
      </main>
    </div>
  );
}

function Metric({
  title,
  value,
  icon,
  critical = false,
}: {
  title: string;
  value: number;
  icon: React.ReactNode;
  critical?: boolean;
}) {
  return (
    <article
      className={`metric-card ${
        critical ? "critical-card" : ""
      }`}
    >
      <div className="metric-icon">
        {icon}
      </div>

      <div>
        <span>{title}</span>
        <strong>{value}</strong>
      </div>
    </article>
  );
}

function StatusBadge({
  status,
}: {
  status: CareCaseStatus;
}) {
  const label = status
    .replace(/([A-Z])/g, " $1")
    .trim();

  return (
    <span
      className={`badge status-${status.toLowerCase()}`}
    >
      {label}
    </span>
  );
}

function PriorityBadge({
  priority,
}: {
  priority: ResultPriority | null;
}) {
  if (!priority) {
    return (
      <span className="priority-none">
        Pending result
      </span>
    );
  }

  return (
    <span
      className={`priority priority-${priority.toLowerCase()}`}
    >
      {priority}
    </span>
  );
}

export default App;
import React, { useCallback, useEffect, useMemo, useState } from "react";
import "./HomePage.scss";
import { Link, useNavigate } from "react-router-dom";
import { attemptsApi } from "../../services/attempts.api";
import type { MyAttempt } from "../../models/my-attempt";
import { useAuth } from "../../context/AuthContext";
import { Paged } from "../../models/pages";

const PAGE_SIZE = 12;

const HomePage: React.FC = () => {
  const { user } = useAuth(); // assuming you expose current user; not required but handy
  const navigate = useNavigate();

  const [items, setItems] = useState<MyAttempt[]>([]);
  const [total, setTotal] = useState(0);
  const [skip, setSkip] = useState(0);
  const [loading, setLoading] = useState(false);
  const [err, setErr] = useState<string | null>(null);
  const [status, setStatus] = useState<"all" | "Completed" | "InProgress">("all");

  const hasMore = useMemo(() => items.length < total, [items.length, total]);

  const load = useCallback(async (reset = false) => {
    setLoading(true);
    setErr(null);
    try {
      const s = reset ? 0 : skip;
      const resp: Paged<MyAttempt> = await attemptsApi.myAttempts(s, PAGE_SIZE, {
        status: status === "all" ? undefined : status
      });
      setTotal(resp.total);
      setSkip(s + resp.items.length);
      setItems(prev => reset ? resp.items : [...prev, ...resp.items]);
    } catch (e: any) {
      setErr(e?.message ?? "Failed to load attempts.");
    } finally {
      setLoading(false);
    }
  }, [skip, status]);

  useEffect(() => {
    // initial and whenever filter changes: reset list
    (async () => {
      setSkip(0);
      await load(true);
    })();
  }, [status, load]);

  const onView = (a: MyAttempt) => {
    if (a.status === "InProgress") {
      navigate(`/quiz/${a.quizId}/answer`);
    } else {
      navigate(`/attempt/${a.attemptId}/result`);
    }
  };

  return (
    <div className="home-page">
      <header className="hp-head">
        <h2 className="hp-title">My results</h2>
        <div className="hp-actions">
          <Link to="/quizzes" className="btn ghost">Browse quizzes</Link>
        </div>
      </header>

      <section className="filters">
        <div className="seg">
          <button
            className={`seg-btn ${status === "all" ? "active" : ""}`}
            onClick={() => setStatus("all")}
          >All</button>
          <button
            className={`seg-btn ${status === "Completed" ? "active" : ""}`}
            onClick={() => setStatus("Completed")}
          >Completed</button>
          <button
            className={`seg-btn ${status === "InProgress" ? "active" : ""}`}
            onClick={() => setStatus("InProgress")}
          >In progress</button>
        </div>
      </section>

      {loading && items.length === 0 && <div className="loading">Loading…</div>}
      {err && <div className="error">{err}</div>}

      {!loading && items.length === 0 && !err && (
        <div className="placeholder">
          <p>No attempts yet.</p>
          <p><Link to="/quizzes" className="btn ghost">Find a quiz</Link></p>
        </div>
      )}

      {!!items.length && (
        <section className="grid">
          {items.map(a => <AttemptCard key={a.attemptId} a={a} onView={() => onView(a)} />)}
        </section>
      )}

      {hasMore && (
        <div className="more">
          <button className="btn" onClick={() => load(false)} disabled={loading}>
            {loading ? "Loading…" : "Load more"}
          </button>
        </div>
      )}
    </div>
  );
};

export default HomePage;

// ---------- Card ----------

const AttemptCard: React.FC<{ a: MyAttempt; onView: () => void }> = ({ a, onView }) => {
  const pct = Math.max(0, Math.min(100, Math.round(a.percentage ?? 0)));
  const dateStr = a.submittedAt
    ? new Date(a.submittedAt).toLocaleString()
    : new Date(a.startedAt).toLocaleString();
  const statusLabel = typeof a.status === "number" ? (a.status === 1 ? "Completed" : "InProgress") : a.status;

  return (
    <article className="card attempt">
      <div className="top">
        <h3 className="name">{a.quizName}</h3>
        <span className={`status pill ${statusLabel === "Completed" ? "ok" : "warn"}`}>{statusLabel}</span>
      </div>

      <div className="meta">
        <span className="dt">{dateStr}</span>
        <span className="pts">{a.totalScore}{a.maxScore ? ` / ${a.maxScore}` : ""} pts</span>
      </div>

      <div className="meter">
        <div className="bar" style={{ width: `${pct}%` }} />
      </div>
      <div className="percent">{pct}%</div>

      <div className="actions">
        <button className="btn primary" onClick={onView}>
          {statusLabel === "Completed" ? "View result" : "Resume"}
        </button>
      </div>
    </article>
  );
};

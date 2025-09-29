import React, { useCallback, useEffect, useMemo, useRef, useState } from "react";
import "./HomePage.scss";
import { Link, useNavigate } from "react-router-dom";
import { attemptsApi } from "../../services/attempts.api";
import type { MyAttempt } from "../../models/my-attempt";
import { useAuth } from "../../context/AuthContext";
import { Paged } from "../../models/pages";

const PAGE_SIZE = 12;

const HomePage: React.FC = () => {
  const { user } = useAuth();
  const navigate = useNavigate();

  const [items, setItems] = useState<MyAttempt[]>([]);
  const [total, setTotal] = useState(0);
  const [skip, setSkip] = useState(0);
  const [loading, setLoading] = useState(false);
  const [err, setErr] = useState<string | null>(null);
  const [status, setStatus] = useState<"all" | "Completed" | "InProgress">("all");

  const requestSeqRef = useRef(0);

  const hasMore = useMemo(() => items.length < total, [items.length, total]);

  const loadPage = useCallback(
    async (offset: number, replace: boolean) => {
      const seq = ++requestSeqRef.current;
      setLoading(true);
      setErr(null);
      try {
        const resp: Paged<MyAttempt> = await attemptsApi.myAttempts(offset, PAGE_SIZE, {
          status: status === "all" ? undefined : status,
        });

        if (seq !== requestSeqRef.current) return;

        setTotal(resp.total);
        setSkip(offset + resp.items.length);
        setItems(prev => (replace ? resp.items : [...prev, ...resp.items]));
      } catch (e: any) {
        if (seq !== requestSeqRef.current) return;
        setErr(e?.message ?? "Failed to load attempts.");
      } finally {
        if (seq === requestSeqRef.current) setLoading(false);
      }
    },
    [status]
  );

  useEffect(() => {
    setItems([]);
    setTotal(0);
    setSkip(0);
    loadPage(0, true);
  }, [status, loadPage]);

  const onView = (a: MyAttempt) => {
    if (a.status === "InProgress") {
      navigate(`/quiz/${a.quizId}/answer`);
    } else {
      navigate(`/attempt/${a.attemptId}/result`);
    }
  };

  const setFilter = (value: "all" | "Completed" | "InProgress") => {
    if (status !== value) setStatus(value);
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
            onClick={() => setFilter("all")}
            disabled={loading}
          >All</button>
          <button
            className={`seg-btn ${status === "Completed" ? "active" : ""}`}
            onClick={() => setFilter("Completed")}
            disabled={loading}
          >Completed</button>
          <button
            className={`seg-btn ${status === "InProgress" ? "active" : ""}`}
            onClick={() => setFilter("InProgress")}
            disabled={loading}
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
          <button className="btn" onClick={() => loadPage(skip, false)} disabled={loading}>
            {loading ? "Loading…" : "Load more"}
          </button>
        </div>
      )}
    </div>
  );
};

export default HomePage;

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

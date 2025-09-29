import React, { useCallback, useEffect, useMemo, useRef, useState } from "react";
import "./QuizAttemptsTable.scss";
import { attemptsApi } from "../../../services/attempts.api";
import type { QuizAttemptRow } from "../../../models/my-attempt";
import { Link, useNavigate } from "react-router-dom";

type Props = {
  quizId: string;
  pageSize?: number;
  status?: "all" | "Completed" | "InProgress";
  userId?: string;
};

const defaultPageSize = 20;

const QuizAttemptsTable: React.FC<Props> = ({ quizId, pageSize = defaultPageSize, status = "all", userId }) => {
  const [rows, setRows] = useState<QuizAttemptRow[]>([]);
  const [total, setTotal] = useState(0);
  const [skip, setSkip] = useState(0);
  const [loading, setLoading] = useState(false);
  const [err, setErr] = useState<string | null>(null);

  const navigate = useNavigate();
  const hasMore = useMemo(() => rows.length < total, [rows.length, total]);
  const seqRef = useRef(0);

  const load = useCallback(async (reset = false) => {
    const mySeq = ++seqRef.current;
    setLoading(true);
    setErr(null);
    try {
      const s = reset ? 0 : skip;
      const resp = await attemptsApi.quizAttempts(
        quizId, s, pageSize,
        { status: status === "all" ? undefined : status, userId }
      );
      if (mySeq !== seqRef.current) return;
      setTotal(resp.total);
      setSkip(s + resp.items.length);
      setRows(prev => reset ? resp.items : [...prev, ...resp.items]);
    } catch (e: any) {
      if (mySeq === seqRef.current) setErr(e?.message ?? "Failed to load attempts.");
    } finally {
      if (mySeq === seqRef.current) setLoading(false);
    }
  }, [quizId, pageSize, status, userId, skip]);

  useEffect(() => { load(true); }, [quizId, status, userId, pageSize, load]);

  return (
    <article className="card attempts">
      <div className="att-head">
        <h3 className="att-title">Attempts</h3>
        <span className="muted">{total} total</span>
      </div>

      {err && <div className="error">{err}</div>}
      {!err && !rows.length && (loading ? <div className="loading">Loading…</div> : <div className="muted">No attempts.</div>)}

      {!!rows.length && (
        <>
          <div className="att-table" role="table" aria-label="Attempts">
            <div className="hdr" role="row">
              <span className="col col-rank">#</span>
              <span className="col col-user">User</span>
              <span className="col col-date">Date</span>
              <span className="col col-score">Score</span>
              <span className="col col-pct">%</span>
              <span className="col col-status">Status</span>
              <span className="col col-act"></span>
            </div>
            {rows.map((r, idx) => {
              const pct = Math.max(0, Math.min(100, Math.round(r.percentage ?? 0)));
              const dateStr = r.submittedAt
                ? new Date(r.submittedAt).toLocaleString()
                : new Date(r.startedAt).toLocaleString();
              const st = typeof r.status === "number" ? (r.status === 1 ? "Completed" : "InProgress") : r.status;
              const score = r.maxScore ? `${r.totalScore}/${r.maxScore}` : `${r.totalScore}`;
              return (
                <div key={r.attemptId} className="row" role="row">
                  <span className="col col-rank">{idx + 1 + (skip - rows.length)}</span>
                  <span className="col col-user">
                    <Avatar username={r.user.username} photo={r.user.photo} /> {r.user.username}
                  </span>
                  <span className="col col-date">{dateStr}</span>
                  <span className="col col-score">{score} pts</span>
                  <span className="col col-pct">{pct}%</span>
                  <span className={`col col-status ${st === "Completed" ? "ok" : "warn"}`}>{st}</span>
                  <span className="col col-act">
                    <button className="btn ghost small" onClick={() => navigate(`/attempt/${r.attemptId}/result`)}>
                      Open
                    </button>
                  </span>
                </div>
              );
            })}
          </div>

          <div className="more">
            <button className="btn" onClick={() => load(false)} disabled={loading || !hasMore}>
              {loading ? "Loading…" : hasMore ? "Load more" : "No more"}
            </button>
          </div>
        </>
      )}
    </article>
  );
};

export default QuizAttemptsTable;

const Avatar: React.FC<{ username: string; photo?: string | null }> = ({ username, photo }) => {
  const initials = (username || "?").trim().slice(0, 2).toUpperCase();
  if (photo) return <img className="avatar" src={photo} alt={username} />;
  return <span className="avatar fallback">{initials}</span>;
};

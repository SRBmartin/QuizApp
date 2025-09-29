import React, { useEffect, useMemo, useRef, useState } from "react";
import "./LeaderboardPage.scss";
import { Link, useParams } from "react-router-dom";
import { type LeaderboardEntry, type LeaderboardResponse } from "../../../models/quiz";
import { quizzesApi } from "../../../services/quizzes.api";

type Period = "all" | "month" | "week";

const LeaderboardPage: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const [period, setPeriod] = useState<Period>("all");
  const [data, setData] = useState<LeaderboardResponse | null>(null);
  const [loading, setLoading] = useState(false);
  const [err, setErr] = useState<string | null>(null);

  const seqRef = useRef(0);

  const load = async (quizId: string, p: Period) => {
    const mySeq = ++seqRef.current;
    setLoading(true);
    setErr(null);
    try {
      const resp = await quizzesApi.getQuizTop(quizId, { period: p, take: 20 });
      if (mySeq === seqRef.current) setData(resp);
    } catch (e: any) {
      if (mySeq === seqRef.current) setErr(e?.message ?? "Failed to load leaderboard.");
    } finally {
      if (mySeq === seqRef.current) setLoading(false);
    }
  };

  useEffect(() => {
    if (!id) return;
    load(id, period);
  }, [id, period]);

  const myInTop = useMemo(() => {
    if (!data?.myEntry || !data?.top) return false;
    return data.top.some(t => t.user.id === data.myEntry!.user.id);
  }, [data]);

  return (
    <div className="lb-page">
      <header className="lb-head">
        <h2 className="lb-title">Leaderboard</h2>
        <div className="lb-actions">
          {data?.quizId && <Link to={`/quiz/${data.quizId}`} className="btn ghost">Back to quiz</Link>}
          <Link to="/quizzes" className="btn ghost">All quizzes</Link>
        </div>
      </header>

      <div className="lb-subhead">
        <div className="lb-quiz-name">{data?.quizName ?? "…"}</div>
        <div className="seg">
          <button className={`seg-btn ${period === "all" ? "active" : ""}`} onClick={() => setPeriod("all")}>All-time</button>
          <button className={`seg-btn ${period === "month" ? "active" : ""}`} onClick={() => setPeriod("month")}>Monthly</button>
          <button className={`seg-btn ${period === "week" ? "active" : ""}`} onClick={() => setPeriod("week")}>Weekly</button>
        </div>
      </div>

      {loading && <div className="loading">Loading…</div>}
      {err && <div className="error">{err}</div>}

      {!!data && (
        <section className="lb-grid">
          <article className="card board">
            <div className="board-head">
              <div className="left">
                <span className="muted">{data.totalParticipants} participants</span>
              </div>
              <div className="right">
                <span className="muted">Max score: <b>{data.maxScore}</b></span>
              </div>
            </div>

            {!data.top?.length && <div className="muted">No results yet.</div>}

            {!!data.top?.length && (
              <ol className="rows">
                {data.top.map(row => <Row key={`${row.user.id}-${row.rank}`} row={row} highlight={data.myEntry?.user.id === row.user.id} />)}
              </ol>
            )}
          </article>

          {data.myEntry && !myInTop && (
            <article className="card mypos">
              <div className="row my">
                <span className="rank">{data.myEntry.rank}</span>
                <Avatar username={data.myEntry.user.username} photo={data.myEntry.user.photo} />
                <span className="uname">{data.myEntry.user.username}</span>
                <span className="score">{data.myEntry.totalScore} pts</span>
                <span className="pct">{data.myEntry.percentage}%</span>
              </div>
              <div className="muted small">Your best result (outside top 20).</div>
            </article>
          )}
        </section>
      )}
    </div>
  );
};

export default LeaderboardPage;

const Row: React.FC<{ row: LeaderboardEntry; highlight?: boolean }> = ({ row, highlight }) => {
  const dt = row.submittedAt ? new Date(row.submittedAt).toLocaleString() : "";
  return (
    <li className={`row ${highlight ? "me" : ""}`}>
      <span className="rank">{row.rank}</span>
      <Avatar username={row.user.username} photo={row.user.photo} />
      <span className="uname">{row.user.username}</span>
      <span className="score">{row.totalScore} pts</span>
      <span className="pct">{row.percentage}%</span>
      <span className="dt muted">{dt}</span>
    </li>
  );
};

const Avatar: React.FC<{ username: string; photo?: string | null }> = ({ username, photo }) => {
  const initials = (username || "?").trim().slice(0, 2).toUpperCase();
  if (photo) return <img className="avatar" src={photo} alt={username} />;
  return <span className="avatar fallback">{initials}</span>;
};

import React, { useEffect, useMemo, useState } from "react";
import "./QuizzesPage.scss";
import { quizzesApi } from "../../services/quizzes.api";
import type { Quiz } from "../../models/quiz";
import { Link, useNavigate } from "react-router-dom";
import { useAuth } from "../../context/AuthContext";

const PAGE_SIZE = 20;

const QuizzesPage: React.FC = () => {
  const { isAdmin } = useAuth();
  const navigate = useNavigate();
  const [data, setData] = useState<Quiz[]>([]);
  const [skip, setSkip] = useState(0);
  const [hasMore, setHasMore] = useState(true);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const load = async (reset = false) => {
    setLoading(true);
    setError(null);
    try {
      const nextSkip = reset ? 0 : skip;
      const page = await quizzesApi.list(nextSkip, PAGE_SIZE);
      const items = page.items ?? [];
      setHasMore(items.length === PAGE_SIZE);
      setSkip(reset ? PAGE_SIZE : nextSkip + items.length);
      setData(reset ? items : [...data, ...items]);
    } catch (e: any) {
      setError(e.message ?? "Failed to load quizzes.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { load(true); }, []);

  const empty = useMemo(() => !loading && data.length === 0, [loading, data]);

  return (
    <div className="quizzes-page">
      <div className="list-head">
        <h2 className="title">Quizzes</h2>
        {isAdmin && (
          <div className="admin-actions">
            <button className="btn primary" onClick={() => navigate("/admin/quizzes")}>
              Manage quizzes
            </button>
            <button className="btn ghost" onClick={() => navigate("/admin/quizzes/new")}>
              Create quiz
            </button>
          </div>
        )}
      </div>

      <div className="quiz-list" role="list">
        {data.map(q => {
          const mins = Math.max(1, Math.round(q.timeInSeconds / 60));
          const count = q.questionCount ?? q.questions?.length ?? 0;
          return (
            <article key={q.id} className="quiz-row" role="listitem">
              <div className="qr-main">
                <h3 className="qr-title">
                  <Link to={`/quiz/${q.id}`}>{q.name}</Link>
                </h3>
                <p className="qr-desc">{q.description ?? "No description."}</p>
                {q.tags?.length ? (
                  <div className="qr-tags">
                    {q.tags.map(t => <span className="tag" key={t.id}>{t.name}</span>)}
                  </div>
                ) : (
                  <div className="qr-tags qr-tags--empty">No tags</div>
                )}
              </div>

              <div className="qr-meta">
                <span className={`pill lvl-${q.difficultyLevel}`} aria-label="Difficulty">
                  {["Easy","Medium","Hard"][q.difficultyLevel] ?? `Level ${q.difficultyLevel}`}
                </span>
                <div className="qr-stats">
                  <span className="mins">{mins} min</span>
                  <span className="count">{count} {count === 1 ? "question" : "questions"}</span>
                </div>
                {isAdmin && (
                  <button className="btn ghost small" onClick={() => navigate(`/admin/quizzes/${q.id}`)}>
                    Open in admin
                  </button>
                )}
              </div>
            </article>
          );
        })}
      </div>

      {empty && <div className="empty">No quizzes yet.</div>}
      {error && <div className="error">{error}</div>}

      <div className="list-foot">
        <button className="btn more" onClick={() => load(false)} disabled={!hasMore || loading}>
          {loading ? "Loading..." : hasMore ? "Load more" : "No more"}
        </button>
      </div>
    </div>
  );
};

export default QuizzesPage;

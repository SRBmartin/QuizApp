import React, { useEffect, useMemo, useRef, useState } from "react";
import "./QuizzesPage.scss";
import { quizzesApi } from "../../services/quizzes.api";
import { tagsApi } from "../../services/tags.api";
import type { Quiz } from "../../models/quiz";
import { Link, useNavigate } from "react-router-dom";
import { useAuth } from "../../context/AuthContext";

const PAGE_SIZE = 20;

const useDebouncedValue = <T,>(value: T, delay = 300) => {
  const [v, setV] = useState(value);
  const t = useRef<number | undefined>(undefined);
  useEffect(() => {
    window.clearTimeout(t.current);
    t.current = window.setTimeout(() => setV(value), delay);
    return () => window.clearTimeout(t.current);
  }, [value, delay]);
  return v;
};

const QuizzesPage: React.FC = () => {
  const { isAdmin } = useAuth();
  const navigate = useNavigate();

  const [data, setData] = useState<Quiz[]>([]);
  const [skip, setSkip] = useState(0);
  const [hasMore, setHasMore] = useState(true);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const [allTags, setAllTags] = useState<{ id: string; name: string }[]>([]);
  const [tagId, setTagId] = useState<string | null>(null);
  const [difficulty, setDifficulty] = useState<number | null>(null); //0=Easy,1=Medium,2=Hard; null = All
  const [search, setSearch] = useState<string>("");
  const debouncedSearch = useDebouncedValue(search, 350);

  const didInit = useRef(false);

  const requestIdRef = useRef(0);

  const load = async (reset = false) => {
    const currentReqId = ++requestIdRef.current;
    setLoading(true);
    setError(null);
    try {
      const nextSkip = reset ? 0 : skip;
      const page = await quizzesApi.list(nextSkip, PAGE_SIZE, {
        tagId,
        difficulty,
        q: debouncedSearch?.trim() ?? null
      });

      if (currentReqId !== requestIdRef.current) return;

      const items = page.items ?? [];
      setHasMore(items.length === PAGE_SIZE);

      setSkip(prev => (reset ? items.length : prev + items.length));
      setData(prev => (reset ? items : [...prev, ...items]));
    } catch (e: any) {
      if (currentReqId === requestIdRef.current) {
        setError(e.message ?? "Failed to load quizzes.");
      }
    } finally {
      if (currentReqId === requestIdRef.current) {
        setLoading(false);
      }
    }
  };

  useEffect(() => {
    (async () => {
      try {
        const tags = await tagsApi.list(0, 200);
        setAllTags(tags);
      } catch {
      }
      await load(true);
      didInit.current = true;
    })();
  }, []);

  useEffect(() => {
    if (!didInit.current) return;
    load(true);
  }, [tagId, difficulty, debouncedSearch]);

  const empty = useMemo(() => !loading && data.length === 0, [loading, data]);

  const diffLabels = ["Easy", "Medium", "Hard"];

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

      <div className="filters">
        <div className="f-row">
          <label className="f-label" htmlFor="quiz-search">Search</label>
          <input
            id="quiz-search"
            className="f-input"
            placeholder="Search title or description…"
            value={search}
            onChange={e => setSearch(e.target.value)}
            maxLength={200}
          />
        </div>

        <div className="f-row">
          <span className="f-label">Tags</span>
          <div className="chip-group scroll-x">
            <button
              type="button"
              className={`chip ${!tagId ? "selected" : ""}`}
              onClick={() => setTagId(null)}
            >
              All
            </button>
            {allTags.map(t => {
              const selected = tagId === t.id;
              return (
                <button
                  type="button"
                  key={t.id}
                  className={`chip ${selected ? "selected" : ""}`}
                  onClick={() => setTagId(selected ? null : t.id)}
                  title={t.name}
                >
                  {t.name}
                </button>
              );
            })}
          </div>
        </div>

        <div className="f-row">
          <span className="f-label">Difficulty</span>
          <div className="chip-group">
            <button
              type="button"
              className={`chip ${difficulty === null ? "selected" : ""}`}
              onClick={() => setDifficulty(null)}
            >
              All
            </button>
            {[0, 1, 2].map(lvl => (
              <button
                type="button"
                key={lvl}
                className={`chip ${difficulty === lvl ? "selected" : ""}`}
                onClick={() => setDifficulty(lvl)}
              >
                {diffLabels[lvl]}
              </button>
            ))}
          </div>
        </div>
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
                <Link className="btn ghost" to={`/quiz/${q.id}/leaderboard`}>Leaderboard</Link>
              </div>
            </article>
          );
        })}
      </div>

      {empty && <div className="empty">No quizzes found.</div>}
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

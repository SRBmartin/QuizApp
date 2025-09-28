import React, { useEffect, useState } from "react";
import "./AdminQuizzesPage.scss";
import { quizzesApi } from "../../../services/quizzes.api";
import type { Quiz } from "../../../models/quiz";
import { Link, useNavigate } from "react-router-dom";

const PAGE_SIZE = 20;

const AdminQuizzesPage: React.FC = () => {
  const navigate = useNavigate();
  const [items, setItems] = useState<Quiz[]>([]);
  const [skip, setSkip] = useState(0);
  const [hasMore, setHasMore] = useState(true);
  const [loading, setLoading] = useState(false);
  const [err, setErr] = useState<string | null>(null);
  const [deletingId, setDeletingId] = useState<string | null>(null);

  const load = async (reset = false) => {
    setLoading(true); setErr(null);
    try {
      const nextSkip = reset ? 0 : skip;
      const page = await quizzesApi.list(nextSkip, PAGE_SIZE);
      const arr = page.items ?? [];
      setHasMore(arr.length === PAGE_SIZE);
      setSkip(reset ? PAGE_SIZE : nextSkip + arr.length);
      setItems(reset ? arr : [...items, ...arr]);
    } catch (e: any) {
      setErr(e.message ?? "Failed to load quizzes.");
    } finally { setLoading(false); }
  };

  useEffect(() => { load(true); /* eslint-disable-next-line react-hooks/exhaustive-deps */ }, []);

  const onDelete = async (id: string) => {
    const q = items.find(x => x.id === id);
    if (!q) return;
    const ok = window.confirm(`Delete quiz "${q.name}"? This cannot be undone.`); // shows confirm dialog
    if (!ok) return;

    const snapshot = items;
    setDeletingId(id);
    // optimistic remove
    setItems(prev => prev.filter(x => x.id !== id));

    try {
      await quizzesApi.delete(id); // HTTP DELETE under the hood
    } catch (e: any) {
      // rollback on failure
      alert(e.message ?? "Failed to delete quiz.");
      setItems(snapshot);
    } finally {
      setDeletingId(null);
    }
  };

  return (
    <div className="admin-quizzes">
      <div className="aq-head">
        <h2>Manage Quizzes</h2>
        <button className="btn primary" onClick={() => navigate("/admin/quizzes/new")}>Create quiz</button>
      </div>

      <div className="aq-table">
        <div className="row head">
          <div className="col name">Title</div>
          <div className="col lvl">Level</div>
          <div className="col pub">Published</div>
          <div className="col actions" />
        </div>

        {items.map(q => (
          <div className="row" key={q.id} data-busy={deletingId === q.id ? "1" : undefined}>
            <div className="col name"><Link to={`/quiz/${q.id}`}>{q.name}</Link></div>
            <div className="col lvl">{["Easy","Medium","Hard"][q.difficultyLevel] ?? q.difficultyLevel}</div>
            <div className="col pub">{q.isPublished ? "Yes" : "No"}</div>
            <div className="col actions">
              <Link className="btn ghost" to={`/admin/quizzes/${q.id}`}>Edit</Link>
              <button
                className="btn danger"
                onClick={() => onDelete(q.id)}
                disabled={deletingId === q.id}
                aria-busy={deletingId === q.id}
              >
                {deletingId === q.id ? "Deleting…" : "Delete"}
              </button>
            </div>
          </div>
        ))}

        {err && <div className="error">{err}</div>}
        {!loading && items.length === 0 && <div className="empty">No quizzes.</div>}
      </div>

      <div className="table-foot">
        <button className="btn more" onClick={() => load(false)} disabled={!hasMore || loading}>
          {loading ? "Loading..." : hasMore ? "Load more" : "No more"}
        </button>
      </div>
    </div>
  );
};

export default AdminQuizzesPage;

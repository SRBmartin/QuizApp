import React, { useEffect, useState } from "react";
import "./QuizPage.scss";
import { useParams } from "react-router-dom";
import { quizzesApi } from "../../services/quizzes.api";
import type { Quiz } from "../../models/quiz";
import { useAuth } from "../../context/AuthContext";
import { Link } from "react-router-dom";

const QuizPage: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const { isAdmin } = useAuth();
  const [quiz, setQuiz] = useState<Quiz | null>(null);
  const [loading, setLoading] = useState(false);
  const [err, setErr] = useState<string | null>(null);

  useEffect(() => {
    if (!id) return;
    (async () => {
      setLoading(true); setErr(null);
      try {
        const q = await quizzesApi.getById(id);
        setQuiz(q);
      } catch (e: any) {
        setErr(e.message ?? "Failed to load quiz.");
      } finally { setLoading(false); }
    })();
  }, [id]);

  if (loading) return <div className="quiz-page"><div className="loading">Loading…</div></div>;
  if (err) return <div className="quiz-page"><div className="error">{err}</div></div>;
  if (!quiz) return null;

  return (
    <div className="quiz-page">
      <header className="qp-head">
        <h2>{quiz.name}</h2>
        <div className="meta">
          <span className="pill">{["Easy","Medium","Hard"][quiz.difficultyLevel] ?? `Level ${quiz.difficultyLevel}`}</span>
          <span className="muted">{Math.round(quiz.timeInSeconds/60)} min</span>
          {isAdmin && <Link className="btn ghost" to={`/admin/quizzes/${quiz.id}`}>Open in admin</Link>}
        </div>
      </header>

      {quiz.description && <p className="desc">{quiz.description}</p>}

      <div className="tags">
        {quiz.tags?.length ? quiz.tags.map(t => <span className="tag" key={t.id}>{t.name}</span>) : <span className="muted">No tags</span>}
      </div>

      <section className="questions">
        <h3>Questions ({quiz.questions?.length ?? 0})</h3>
        {!quiz.questions?.length && <div className="muted">No questions yet.</div>}
        {/* You can render a preview table here if you want */}
      </section>
    </div>
  );
};

export default QuizPage;

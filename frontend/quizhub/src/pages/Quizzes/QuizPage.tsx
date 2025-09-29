import React, { useEffect, useState } from "react";
import "./QuizPage.scss";
import { useParams, Link, useNavigate } from "react-router-dom";
import { quizzesApi } from "../../services/quizzes.api";
import type { Quiz } from "../../models/quiz";
import { useAuth } from "../../context/AuthContext";
import { QuestionType } from "../../models/question";
import { attemptsApi } from "../../services/attempts.api";

const QuizPage: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const { isAdmin } = useAuth();
  const [quiz, setQuiz] = useState<Quiz | null>(null);
  const [loading, setLoading] = useState(false);
  const [err, setErr] = useState<string | null>(null);
  const navigate = useNavigate();

  useEffect(() => {
    if (!id) return;
    (async () => {
      setLoading(true);
      setErr(null);
      try {
        const q = await quizzesApi.getById(id);
        setQuiz(q);
      } catch (e: any) {
        setErr(e.message ?? "Failed to load quiz.");
      } finally {
        setLoading(false);
      }
    })();
  }, [id]);

  const onStart = async () => {
    if (!quiz) return;
    navigate(`/quiz/${quiz.id}/answer`, { replace: true });
  };

  if (loading) return <div className="quiz-page"><div className="loading">Loading…</div></div>;
  if (err) return <div className="quiz-page"><div className="error">{err}</div></div>;
  if (!quiz) return null;

  const minutes = Math.max(1, Math.round(quiz.timeInSeconds / 60));

  return (
    <div className="quiz-page">
      <header className="qp-head">
        <h2 className="qp-title">{quiz.name}</h2>
        <div className="meta">
          <span className="pill">
            {["Easy","Medium","Hard"][quiz.difficultyLevel] ?? `Level ${quiz.difficultyLevel}`}
          </span>
          <span className="muted">{minutes} min</span>
          {isAdmin && <Link className="btn ghost" to={`/admin/quizzes/${quiz.id}`}>Open in admin</Link>}
        </div>
      </header>

      {quiz.description && <p className="desc">{quiz.description}</p>}

      <div className="tags">
        {quiz.tags?.length
          ? quiz.tags.map(t => <span className="tag" key={t.id}>{t.name}</span>)
          : <span className="muted">No tags</span>}
      </div>

      {!isAdmin && (
        <div className="cta">
          <button type="button" className="btn primary start-btn" onClick={onStart}>
            Start the quiz
          </button>
        </div>
      )}

      {isAdmin && (
        <section className="questions">
          <h3 className="qs-title">Questions</h3>
          {!quiz.questions?.length && <div className="muted">No questions yet.</div>}

          {!!quiz.questions?.length && (
            <div className="q-list">
              {quiz.questions.map((q) => (
                <article key={q.id} className="q-item">
                  <div className="q-top">
                    <span className="q-type">{QuestionType[q.type]}</span>
                    <span className="q-points">{q.points} pts</span>
                  </div>
                  <div className="q-text">{q.question}</div>

                  {(q.type === QuestionType.Single || q.type === QuestionType.Multi || q.type === QuestionType.TrueFalse) && q.choices?.length > 0 && (
                    <ul className="q-choices">
                      {q.choices.map(c => <li key={c.id ?? c.label}>{c.label}</li>)}
                    </ul>
                  )}

                  {q.type === QuestionType.FillIn && (
                    <div className="q-fill">
                      <span className="blank">__________</span>
                    </div>
                  )}
                </article>
              ))}
            </div>
          )}
        </section>
      )}
    </div>
  );
};

export default QuizPage;

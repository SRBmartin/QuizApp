import React, { useEffect, useMemo, useState } from "react";
import "./ResultPage.scss";
import { Link, useNavigate, useParams } from "react-router-dom";
import { attemptsApi } from "../../services/attempts.api";
import type { AttemptResultSummary, AttemptResultReview, AttemptReviewItem } from "../../models/attempt-result";
import { QuestionType } from "../../models/question";
import type { AttemptState } from "../../models/attempt";

const ResultPage: React.FC = () => {
  const { attemptId } = useParams<{ attemptId: string }>();
  const navigate = useNavigate();

  const [summary, setSummary] = useState<AttemptResultSummary | null>(null);
  const [review, setReview] = useState<AttemptResultReview | null>(null);
  const [loading, setLoading] = useState(false);
  const [err, setErr] = useState<string | null>(null);

  const pct = useMemo(() => {
    if (!summary) return 0;
    if (Number.isFinite(summary.percentage)) return Math.max(0, Math.min(100, Math.round(summary.percentage)));
    if (summary.maxScore > 0) return Math.round((summary.totalScore / summary.maxScore) * 100);
    if (summary.totalQuestions > 0) return Math.round((summary.correctAnswers / summary.totalQuestions) * 100);
    return 0;
  }, [summary]);

  const durationStr = useMemo(() => {
    if (!summary?.durationSeconds && summary?.startedAt && summary?.submittedAt) {
      const s = Math.max(0, Math.floor((new Date(summary.submittedAt).getTime() - new Date(summary.startedAt).getTime()) / 1000));
      const m = Math.floor(s / 60).toString().padStart(2, "0");
      const ss = (s % 60).toString().padStart(2, "0");
      return `${m}:${ss}`;
    }
    const s = Math.max(0, summary?.durationSeconds ?? 0);
    const m = Math.floor(s / 60).toString().padStart(2, "0");
    const ss = (s % 60).toString().padStart(2, "0");
    return `${m}:${ss}`;
  }, [summary]);

  useEffect(() => {
    if (!attemptId) return;
    let cancelled = false;

    (async () => {
      setLoading(true);
      setErr(null);
      try {
        // 1) Try explicit summary endpoint
        let sum: AttemptResultSummary | null = null;
        try {
          sum = await attemptsApi.resultSummary(attemptId);
        } catch {
          const st = await attemptsApi.getState(attemptId);
          sum = mapStateToSummary(st);
        }

        if (sum && (sum.status === "InProgress" || (typeof sum.status === "number" && sum.status !== 1))) {
          navigate(`/quiz/${sum.quizId}/answer`, { replace: true });
          return;
        }

        if (!cancelled) setSummary(sum);

        let rev: AttemptResultReview | null = null;
        try {
          rev = await attemptsApi.resultReview(attemptId);
        } catch {
          try {
            const combined = await attemptsApi.resultCombined?.(attemptId);
            if (combined) {
              rev = { attemptId, items: combined.items ?? [] };
              if (combined.summary && !cancelled) setSummary(combined.summary);
            }
          } catch {
            /*ignore – show summary only*/
          }
        }

        if (!cancelled) setReview(rev);
      } catch (e: any) {
        if (!cancelled) setErr(e?.message ?? "Failed to load results.");
      } finally {
        if (!cancelled) setLoading(false);
      }
    })();

    return () => { cancelled = true; };
  }, [attemptId, navigate]);

  if (!attemptId) return null;

  return (
    <div className="result-page">
      <header className="rp-head">
        <h2 className="rp-title">Results</h2>

        <div className="rp-actions">
          <Link to="/quizzes" className="btn ghost">All quizzes</Link>
          {summary?.quizId && (
            <Link to={`/quiz/${summary.quizId}`} className="btn ghost">Back to quiz</Link>
          )}
        </div>
      </header>

      {loading && <div className="loading">Loading…</div>}
      {err && <div className="error">{err}</div>}

      {!loading && summary && (
        <section className="res-grid">

          <article className="card score">
            <div className="score-top">
              <div className="big">{pct}<span className="unit">%</span></div>
              <div className="meter">
                <div className="bar" style={{ width: `${pct}%` }} />
              </div>
            </div>
            <div className="score-meta">
              <div className="stat">
                <span className="label">Correct</span>
                <span className="val">{summary.correctAnswers}/{summary.totalQuestions}</span>
              </div>
              <div className="stat">
                <span className="label">Points</span>
                <span className="val">{summary.totalScore}{summary.maxScore ? ` / ${summary.maxScore}` : ""}</span>
              </div>
              <div className="stat">
                <span className="label">Duration</span>
                <span className="val">{durationStr}</span>
              </div>
              {Number.isFinite(summary.timeLimitSeconds) && summary.timeLimitSeconds > 0 && (
                <div className="stat">
                  <span className="label">Time limit</span>
                  <span className="val">{Math.ceil((summary.timeLimitSeconds ?? 0) / 60)} min</span>
                </div>
              )}
            </div>
          </article>

          <article className="card review">
            <div className="rev-head">
              <h3 className="rev-title">Answer review</h3>
              <span className="pill">{summary.correctAnswers} correct / {summary.totalQuestions} total</span>
            </div>

            {!review?.items?.length && (
              <div className="muted">Detailed review is not available.</div>
            )}

            {!!review?.items?.length && (
              <div className="rev-list">
                {review.items.map((it, idx) => (
                  <ReviewItem key={it.questionId ?? idx} item={it} index={idx + 1} />
                ))}
              </div>
            )}
          </article>
        </section>
      )}
    </div>
  );
};

export default ResultPage;

function mapStateToSummary(st: AttemptState): AttemptResultSummary {
  const percentage = st.totalQuestions > 0
    ? Math.round((st.totalScore / Math.max(st.totalQuestions, 1)) * 100)
    : 0;

  return {
    attemptId: st.id,
    quizId: st.quizId,
    quizName: "",
    status: st.status,
    totalQuestions: st.totalQuestions,
    correctAnswers: st.totalScore,
    totalScore: st.totalScore,
    maxScore: st.totalQuestions,
    percentage,
    startedAt: st.startedAt,
    submittedAt: st.submittedAt ?? undefined,
    durationSeconds: Math.max(0, ((new Date(st.submittedAt ?? st.startedAt).getTime() - new Date(st.startedAt).getTime()) / 1000) | 0),
    timeLimitSeconds: st.timeLimitSeconds ?? 0
  };
}

const ReviewItem: React.FC<{ item: AttemptReviewItem; index: number }> = ({ item, index }) => {
  const t = (typeof item.type === "number" ? QuestionType[item.type] : item.type) ?? "";
  const cls = item.isCorrect ? "q ok" : "q bad";

  return (
    <article className={cls}>
      <div className="q-top">
        <span className="idx">Q{index}</span>
        <span className="type pill">{t}</span>
        <span className="pts">{item.awardedScore ?? 0}/{item.points ?? 0} pts</span>
      </div>

      <div className="q-text">{item.question}</div>

      {(item.type === QuestionType.Single || item.type === QuestionType.TrueFalse || item.type === QuestionType.Multi) && !!item.options?.length && (
        <ul className={`opts ${item.type === QuestionType.Multi ? "multi" : "single"}`}>
          {item.options.map(o => {
            const chosen = !!o.selectedByUser;
            const correct = !!o.isCorrect;
            const liCls =
              correct && chosen ? "opt hit"
              : correct && !chosen ? "opt correct"
              : !correct && chosen ? "opt miss"
              : "opt";

            return (
              <li key={o.id} className={liCls}>
                <span className="box" />
                <span className="lbl">{o.text}</span>
              </li>
            );
          })}
        </ul>
      )}

      {item.type === QuestionType.FillIn && (
        <div className="fill">
          <div className="row">
            <span className="label">Your answer:</span>
            <span className={`val ${item.isCorrect ? "ok" : "bad"}`}>
              {item.textAnswer?.submittedText ?? <em>—</em>}
            </span>
          </div>
          {!!item.textAnswer?.expectedText && !item.isCorrect && (
            <div className="row">
              <span className="label">Correct:</span>
              <span className="val correct">{item.textAnswer.expectedText}</span>
            </div>
          )}
        </div>
      )}
    </article>
  );
};

import React, { useCallback, useEffect, useMemo, useRef, useState } from "react";
import "./AnswerQuizPage.scss";
import { useNavigate, useParams } from "react-router-dom";
import { attemptsApi } from "../../services/attempts.api";
import type { AttemptQuestion } from "../../models/attempt";
import { QuestionType } from "../../models/question";

const SEC = 1000;

const AnswerQuizPage: React.FC = () => {
  const { quizId } = useParams<{ quizId: string }>();
  const navigate = useNavigate();

  const [attemptId, setAttemptId] = useState<string | null>(null);
  const [question, setQuestion] = useState<AttemptQuestion | null>(null);
  const [loading, setLoading] = useState(false);
  const [saving, setSaving] = useState(false);
  const [err, setErr] = useState<string | null>(null);

  const [timeLeft, setTimeLeft] = useState<number>(0);
  const [limit, setLimit] = useState<number>(0);

  // ✅ store selection/text in state so UI re-renders and button enables
  const [selectedIds, setSelectedIds] = useState<Set<string>>(new Set());
  const [textAnswer, setTextAnswer] = useState<string>("");

  const tickRef = useRef<number | null>(null);
  const resyncRef = useRef<number | null>(null);
  const submittingRef = useRef(false);
  const initOnceRef = useRef(false);

  const STORAGE_KEY = useMemo(() => (quizId ? `attempt:${quizId}` : "attempt:?"), [quizId]);

  const clearTimers = useCallback(() => {
    if (tickRef.current) window.clearInterval(tickRef.current);
    if (resyncRef.current) window.clearInterval(resyncRef.current);
    tickRef.current = null;
    resyncRef.current = null;
  }, []);

  const startTicking = useCallback((attempt: string, left: number, lim: number) => {
    const safeLeft = Number.isFinite(left) ? left : 0;
    const safeLim = Number.isFinite(lim) ? lim : 0;

    setTimeLeft(safeLeft);
    setLimit(safeLim);

    clearTimers();

    tickRef.current = window.setInterval(() => {
      setTimeLeft(prev => {
        const p = Number.isFinite(prev) ? prev : 0;
        const next = Math.max(0, p - 1);
        if (next === 0 && !submittingRef.current) {
          submittingRef.current = true;
          attemptsApi.submit(attempt).finally(() => {
            navigate(`/attempt/${attempt}/result`, { replace: true });
          });
        }
        return next;
      });
    }, SEC);

    resyncRef.current = window.setInterval(async () => {
      try {
        const s = await attemptsApi.getState(attempt);
        setTimeLeft(Math.max(0, s.timeLeftSeconds ?? 0));
        setLimit(s.timeLimitSeconds ?? 0);
        if (s.status === "Completed" || (s.timeLeftSeconds ?? 0) <= 0) {
          navigate(`/attempt/${attempt}/result`, { replace: true });
        }
      } catch {/* ignore */}
    }, 20000);
  }, [clearTimers, navigate]);

  const init = useCallback(async () => {
    if (!quizId) return;
    setLoading(true);
    setErr(null);
    try {
      // Start once; backend resumes or creates one active attempt
      const resp = await attemptsApi.start(quizId);
      setAttemptId(resp.attemptId);
      localStorage.setItem(STORAGE_KEY, JSON.stringify({ attemptId: resp.attemptId }));
      setQuestion(resp.question);
      // reset selections for first question
      setSelectedIds(new Set());
      setTextAnswer("");
      startTicking(resp.attemptId, resp.timeLeftSeconds, resp.timeLimitSeconds);
    } catch (e: any) {
      setErr(e?.message ?? "Failed to start the attempt.");
    } finally {
      setLoading(false);
    }
  }, [STORAGE_KEY, quizId, startTicking]);

  useEffect(() => {
    if (initOnceRef.current) return;
    initOnceRef.current = true;
    void init();

    const onVis = () => {
      if (document.visibilityState !== "visible" || !attemptId) return;
      attemptsApi.getState(attemptId).then(s => {
        setTimeLeft(Math.max(0, s.timeLeftSeconds ?? 0));
        setLimit(s.timeLimitSeconds ?? 0);
        if (s.status === "Completed" || (s.timeLeftSeconds ?? 0) <= 0) {
          navigate(`/attempt/${attemptId}/result`, { replace: true });
        }
      }).catch(() => {});
    };

    document.addEventListener("visibilitychange", onVis);
    return () => {
      document.removeEventListener("visibilitychange", onVis);
      clearTimers();
    };
  }, [attemptId, clearTimers, init, navigate]);

  // ===== Selection handlers (STATE, not ref) =====

  const onOptionToggle = (id: string) => {
    if (!question) return;
    setSelectedIds(prev => {
      if (question.type === QuestionType.Multi) {
        const ns = new Set(prev);
        ns.has(id) ? ns.delete(id) : ns.add(id);
        return ns;
      }
      // Single / TrueFalse
      return new Set([id]);
    });
  };

  const onTextChange = (v: string) => setTextAnswer(v);

  const canSubmitAnswer = useMemo(() => {
    if (!question) return false;
    if (question.type === QuestionType.FillIn) {
      return !!textAnswer.trim();
    }
    if (question.type === QuestionType.Multi) {
      return selectedIds.size > 0;
    }
    return selectedIds.size === 1; // Single, TrueFalse
  }, [question, textAnswer, selectedIds]);

  // ==============================================

  const onSaveAnswer = async () => {
    if (!attemptId || !question || !canSubmitAnswer) return;

    setSaving(true);
    setErr(null);
    try {
      const payload: any = {};
      if (question.type === QuestionType.FillIn) {
        payload.submittedText = textAnswer.trim();
      } else {
        payload.selectedChoiceIds = Array.from(selectedIds);
      }

      const next = await attemptsApi.saveAnswer(attemptId, question.id, payload);
      // move to next question, reset UI selection
      setQuestion(next);
      setSelectedIds(new Set());
      setTextAnswer("");
    } catch (e: any) {
      const msg = e?.message as string;
      if (msg?.toLowerCase().includes("time")) {
        navigate(`/attempt/${attemptId}/result`, { replace: true });
      } else {
        setErr(msg ?? "Failed to save answer.");
      }
    } finally {
      setSaving(false);
    }
  };

  const onFinishNow = async () => {
    if (!attemptId) return;
    try {
      submittingRef.current = true;
      await attemptsApi.submit(attemptId);
      navigate(`/attempt/${attemptId}/result`, { replace: true });
    } catch (e: any) {
      setErr(e.message ?? "Failed to submit attempt.");
      submittingRef.current = false;
    }
  };

  const safeLeft = Number.isFinite(timeLeft) ? timeLeft : 0;
  const safeLimit = Number.isFinite(limit) ? limit : 0;
  const mm = Math.floor(safeLeft / 60).toString().padStart(2, "0");
  const ss = (safeLeft % 60).toString().padStart(2, "0");
  const typeLabel = question ? (typeof question.type === "number" ? QuestionType[question.type] : String(question.type)) : "";

  return (
    <div className="answer-page">
      <header className="ap-head">
        <h2 className="ap-title">Quiz in progress</h2>
        <div className="ap-meta">
          <span className="timer">⏱ {mm}:{ss}</span>
          <span className="muted">{Math.ceil(safeLimit / 60)} min total</span>
          <button type="button" className="btn ghost" onClick={onFinishNow} disabled={saving || !attemptId}>Finish now</button>
        </div>
      </header>

      {loading && <div className="loading">Loading…</div>}
      {err && <div className="error">{err}</div>}

      {!loading && question && (
        <section className="q-card">
          <div className="q-head">
            <span className="q-type pill">{typeLabel}</span>
            <span className="q-points">{question.points} pts</span>
          </div>

          <div className="q-text">{question.question}</div>

          {(question.type === QuestionType.Single || question.type === QuestionType.TrueFalse) && (
            <ul className="opt-list single">
              {question.options.map(o => (
                <li key={o.id}>
                  <label className="opt">
                    <input
                      type="radio"
                      name="opt"
                      checked={selectedIds.has(o.id)}
                      onChange={() => onOptionToggle(o.id)}
                    />
                    <span className="opt-label">{o.text}</span>
                  </label>
                </li>
              ))}
            </ul>
          )}

          {question.type === QuestionType.Multi && (
            <ul className="opt-list multi">
              {question.options.map(o => (
                <li key={o.id}>
                  <label className="opt">
                    <input
                      type="checkbox"
                      checked={selectedIds.has(o.id)}
                      onChange={() => onOptionToggle(o.id)}
                    />
                    <span className="opt-label">{o.text}</span>
                  </label>
                </li>
              ))}
            </ul>
          )}

          {question.type === QuestionType.FillIn && (
            <div className="text-answer">
              <input
                type="text"
                placeholder="Type your answer…"
                value={textAnswer}
                onChange={(e) => onTextChange(e.target.value)}
                onKeyDown={(e) => {
                  if (e.key === "Enter" && canSubmitAnswer && !saving) onSaveAnswer();
                }}
              />
            </div>
          )}

          <div className="actions">
            <button type="button" className="btn primary" onClick={onSaveAnswer} disabled={!canSubmitAnswer || saving}>
              {question.isLast ? "Save answer" : "Save & next"}
            </button>
          </div>
        </section>
      )}
    </div>
  );
};

export default AnswerQuizPage;

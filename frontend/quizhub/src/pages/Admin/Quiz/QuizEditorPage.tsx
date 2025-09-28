// src/pages/Admin/Quiz/QuizEditorPage.tsx
import React, { useEffect, useMemo, useState } from "react";
import "./QuizEditorPage.scss";
import { useNavigate, useParams } from "react-router-dom";
import { quizzesApi } from "../../../services/quizzes.api";
import { tagsApi } from "../../../services/tags.api";
import { questionsApi } from "../../../services/questions.api";
import { QuestionType, Question } from "../../../models/question";
import type { Quiz } from "../../../models/quiz";

type FormState = {
  name: string;
  description?: string;
  difficultyLevel: number;
  timeInSeconds: number;
  isPublished: boolean;
  tagIds: string[];
};

const emptyForm: FormState = {
  name: "",
  description: "",
  difficultyLevel: 0,
  timeInSeconds: 300,
  isPublished: false,
  tagIds: [],
};

const QuizEditorPage: React.FC = () => {
  const { quizId } = useParams<{ quizId: string }>();
  const navigate = useNavigate();
  const isCreate = !quizId;

  const [form, setForm] = useState<FormState>(emptyForm);
  const [allTags, setAllTags] = useState<{ id: string; name: string }[]>([]);
  const [loading, setLoading] = useState(false);
  const [saving, setSaving] = useState(false);
  const [err, setErr] = useState<string | null>(null);

  const [questions, setQuestions] = useState<Question[]>([]);
  const [editingId, setEditingId] = useState<string | null>(null);

  const [qForm, setQForm] = useState<{
    question: string;
    points: number;
    type: QuestionType;
    choices: { id?: string; label: string; isCorrect: boolean }[];
    isTrueCorrect: boolean | null;
    textAnswer: string;
  }>({
    question: "",
    points: 1,
    type: QuestionType.Single,
    choices: [
      { label: "", isCorrect: false },
      { label: "", isCorrect: false }
    ],
    isTrueCorrect: null,
    textAnswer: ""
  });

  const [qSaving, setQSaving] = useState(false);
  const [qErr, setQErr] = useState<string | null>(null);

  useEffect(() => {
    (async () => {
      setLoading(true); setErr(null);
      try {
        const tags = await tagsApi.list(0, 200);
        setAllTags(tags);
        if (!isCreate && quizId) {
          const q: Quiz = await quizzesApi.getById(quizId);
          const initial: FormState = {
            name: q.name,
            description: q.description ?? "",
            difficultyLevel: q.difficultyLevel,
            timeInSeconds: q.timeInSeconds,
            isPublished: q.isPublished,
            tagIds: q.tags?.map(t => t.id) ?? [],
          };
          setForm(initial);
          setQuestions(q.questions ?? []);
        } else {
          setForm(emptyForm);
          setQuestions([]);
        }
      } catch (e: any) {
        setErr(e.message ?? "Failed to load data.");
      } finally { setLoading(false); }
    })();
  }, [quizId, isCreate]);

  const canSave = useMemo(() => form.name.trim().length >= 2 && form.timeInSeconds > 0, [form]);

  const onSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!canSave) return;
    setSaving(true); setErr(null);
    try {
      if (isCreate) {
        const created = await quizzesApi.create({
          name: form.name.trim(),
          description: form.description?.trim() || undefined,
          difficultyLevel: form.difficultyLevel,
          timeInSeconds: form.timeInSeconds,
        });
        if (form.tagIds.length) await quizzesApi.updateTags(created.id, form.tagIds);
        navigate(`/admin/quizzes/${created.id}`, { replace: true });
      } else if (quizId) {
        const updated = await quizzesApi.update(quizId, {
          name: form.name.trim(),
          description: form.description?.trim() || undefined,
          difficultyLevel: form.difficultyLevel,
          timeInSeconds: form.timeInSeconds,
          isPublished: form.isPublished,
        });
        await quizzesApi.updateTags(updated.id, form.tagIds);
        navigate(`/admin/quizzes/${updated.id}`);
      }
    } catch (e: any) {
      setErr(e.message ?? "Failed to save quiz.");
    } finally { setSaving(false); }
  };

  const onQTypeChange = (t: QuestionType) => {
    setQErr(null);
    setQForm(prev => ({
      question: prev.question,
      points: prev.points,
      type: t,
      choices:
        t === QuestionType.Single || t === QuestionType.Multi
          ? (prev.type === QuestionType.Single || prev.type === QuestionType.Multi)
            ? prev.choices.length >= 2 ? prev.choices : [
                { label: "", isCorrect: false },
                { label: "", isCorrect: false }
              ]
            : [
                { label: "", isCorrect: false },
                { label: "", isCorrect: false }
              ]
          : [],
      isTrueCorrect: t === QuestionType.TrueFalse ? (prev.type === QuestionType.TrueFalse ? prev.isTrueCorrect : false) : null,
      textAnswer: t === QuestionType.FillIn ? (prev.type === QuestionType.FillIn ? prev.textAnswer : "") : ""
    }));
  };

  const canCreateQuestion = useMemo(() => {
    if (!quizId) return false;
    const q = qForm.question.trim().length >= 3;
    const p = Number.isFinite(qForm.points) && qForm.points >= 0;
    if (!(q && p)) return false;

    if (qForm.type === QuestionType.Single || qForm.type === QuestionType.Multi) {
      const labelsOk = qForm.choices.length >= 2 && qForm.choices.every(c => c.label.trim().length > 0);
      const correctCount = qForm.choices.filter(c => c.isCorrect).length;
      if (qForm.type === QuestionType.Single) return labelsOk && correctCount === 1;
      return labelsOk && correctCount >= 1;
    }
    if (qForm.type === QuestionType.TrueFalse) return qForm.isTrueCorrect !== null;
    if (qForm.type === QuestionType.FillIn) return qForm.textAnswer.trim().length > 0;
    return false;
  }, [qForm, quizId]);

  const resetQFormForCreate = () => {
    setEditingId(null);
    setQForm(prev => ({
      question: "",
      points: 1,
      type: QuestionType.Single,
      choices: [
        { label: "", isCorrect: false },
        { label: "", isCorrect: false }
      ],
      isTrueCorrect: null,
      textAnswer: ""
    }));
  };

  const startEdit = (q: Question) => {
    setEditingId(q.id);
    if (q.type === QuestionType.TrueFalse) {
      const trueIsCorrect = !!q.choices.find(c => c.label === "True" && c.isCorrect);
      setQForm({
        question: q.question,
        points: q.points,
        type: QuestionType.TrueFalse,
        choices: [],
        isTrueCorrect: trueIsCorrect,
        textAnswer: ""
      });
    } else if (q.type === QuestionType.FillIn) {
      setQForm({
        question: q.question,
        points: q.points,
        type: QuestionType.FillIn,
        choices: [],
        isTrueCorrect: null,
        textAnswer: q.textAnswer?.text ?? ""
      });
    } else {
      setQForm({
        question: q.question,
        points: q.points,
        type: q.type,
        choices: q.choices.map(c => ({ id: c.id, label: c.label, isCorrect: c.isCorrect })),
        isTrueCorrect: null,
        textAnswer: ""
      });
    }
  };

  const submitQuestion = async () => {
    if (!quizId || !canCreateQuestion) return;
    setQSaving(true); setQErr(null);
    try {
      if (!editingId) {
        const payload: any = {
          question: qForm.question.trim(),
          points: qForm.points,
          questionType: qForm.type
        };
        if (qForm.type === QuestionType.Single || qForm.type === QuestionType.Multi) {
          payload.choices = qForm.choices.map(c => ({ label: c.label.trim(), isCorrect: c.isCorrect }));
        } else if (qForm.type === QuestionType.TrueFalse) {
          payload.isTrueCorrect = !!qForm.isTrueCorrect;
        } else if (qForm.type === QuestionType.FillIn) {
          payload.textAnswer = qForm.textAnswer.trim();
        }
        const created = await questionsApi.create(quizId, payload);
        setQuestions(prev => [created, ...prev]);
        resetQFormForCreate();
      } else {
        const payload: any = {
          question: qForm.question.trim(),
          points: qForm.points
        };
        if (qForm.type === QuestionType.Single || qForm.type === QuestionType.Multi) {
          payload.choices = qForm.choices.map(c => ({ id: c.id, label: c.label.trim(), isCorrect: c.isCorrect }));
        } else if (qForm.type === QuestionType.TrueFalse) {
          payload.isTrueCorrect = !!qForm.isTrueCorrect;
        } else if (qForm.type === QuestionType.FillIn) {
          payload.textAnswer = qForm.textAnswer.trim();
        }
        const updated = await questionsApi.update(editingId, payload);
        setQuestions(prev => prev.map(x => (x.id === updated.id ? updated : x)));
        resetQFormForCreate();
      }
    } catch (e: any) {
      setQErr(e.message ?? "Failed to save question.");
    } finally {
      setQSaving(false);
    }
  };

  const onDeleteQuestion = async (id: string) => {
    if (!quizId) return;
    if (!window.confirm("Delete this question?")) return;
    try {
      await questionsApi.delete(id);
      setQuestions(prev => prev.filter(x => x.id !== id));
      if (editingId === id) resetQFormForCreate();
    } catch (e: any) {
      setQErr(e.message ?? "Failed to delete question.");
    }
  };

  return (
    <div className="quiz-editor">
      <header className="qe-head">
        <h2>{isCreate ? "Create quiz" : "Edit quiz"}</h2>
      </header>

      {loading && <div className="loading">Loading…</div>}

      {!!err && (
        <div className="error-banner" role="alert" aria-live="assertive" aria-atomic="true">
          <span className="error-icon" aria-hidden>⚠️</span>
          <span className="error-text">{err}</span>
          <button className="error-dismiss" type="button" onClick={() => setErr(null)} aria-label="Dismiss error">×</button>
        </div>
      )}

      <form className="qe-form" onSubmit={onSubmit} aria-describedby={err ? "qe-error" : undefined}>
        <label>
          <span>Title</span>
          <input
            value={form.name}
            onChange={e => { setForm({ ...form, name: e.target.value }); if (err) setErr(null); }}
            placeholder="e.g. European Capitals"
            maxLength={128}
            required
          />
        </label>

        <label>
          <span>Description</span>
          <textarea
            value={form.description}
            onChange={e => { setForm({ ...form, description: e.target.value }); if (err) setErr(null); }}
            placeholder="Short summary"
            maxLength={1024}
          />
        </label>

        <label>
          <span>Difficulty</span>
          <select
            value={form.difficultyLevel}
            onChange={e => { setForm({ ...form, difficultyLevel: Number(e.target.value) }); if (err) setErr(null); }}>
            <option value={0}>Easy</option>
            <option value={1}>Medium</option>
            <option value={2}>Hard</option>
          </select>
        </label>

        <label>
          <span>Time (seconds)</span>
          <input
            type="number" min={30} step={30}
            value={form.timeInSeconds}
            onChange={e => { setForm({ ...form, timeInSeconds: Number(e.target.value) }); if (err) setErr(null); }}
          />
        </label>

        {!isCreate && (
          <label className="inline">
            <input
              type="checkbox"
              checked={form.isPublished}
              onChange={e => { setForm({ ...form, isPublished: e.target.checked }); if (err) setErr(null); }} />
            <span>Published</span>
          </label>
        )}

        <label>
          <span>Tags</span>
          <div className="tags-picker">
            {allTags.map(t => {
              const selected = form.tagIds.includes(t.id);
              return (
                <button
                  type="button"
                  key={t.id}
                  className={`chip ${selected ? "selected" : ""}`}
                  onClick={() => {
                    setForm(f => ({
                      ...f,
                      tagIds: selected ? f.tagIds.filter(x => x !== t.id) : [...f.tagIds, t.id],
                    }));
                    if (err) setErr(null);
                  }}
                >
                  {t.name}
                </button>
              );
            })}
          </div>
        </label>

        <div className="actions">
          <button className="btn primary" type="submit" disabled={!canSave || saving}>
            {saving ? "Saving…" : "Save"}
          </button>
          <button className="btn ghost" type="button" onClick={() => navigate(-1)}>
            Cancel
          </button>
        </div>
      </form>

      {!isCreate && (
        <section className="qe-questions">
          <div className="q-head">
            <h3>{editingId ? "Edit Question" : "Add Question"}</h3>
          </div>

          {!!qErr && (
            <div className="error-banner" role="alert" aria-live="assertive" aria-atomic="true">
              <span className="error-icon" aria-hidden>⚠️</span>
              <span className="error-text">{qErr}</span>
              <button className="error-dismiss" type="button" onClick={() => setQErr(null)} aria-label="Dismiss error">×</button>
            </div>
          )}

          <div className="q-form" aria-disabled={form.isPublished ? "true" : undefined}>
            <label>
              <span>Question</span>
              <input
                value={qForm.question}
                onChange={e => { setQForm({ ...qForm, question: e.target.value }); if (qErr) setQErr(null); }}
                placeholder="Type the question…"
                maxLength={1024}
                disabled={form.isPublished}
              />
            </label>

            <div className="row2">
              <label>
                <span>Points</span>
                <input
                  type="number" min={0} step={1}
                  value={qForm.points}
                  onChange={e => { setQForm({ ...qForm, points: Number(e.target.value) || 0 }); if (qErr) setQErr(null); }}
                  disabled={form.isPublished}
                />
              </label>

              <label>
                <span>Type</span>
                <select
                  value={qForm.type}
                  onChange={e => onQTypeChange(Number(e.target.value) as QuestionType)}
                  disabled={form.isPublished || !!editingId}
                >
                  <option value={QuestionType.Single}>Single choice</option>
                  <option value={QuestionType.Multi}>Multiple choice</option>
                  <option value={QuestionType.TrueFalse}>True / False</option>
                  <option value={QuestionType.FillIn}>Fill in</option>
                </select>
              </label>
            </div>

            {(qForm.type === QuestionType.Single || qForm.type === QuestionType.Multi) && (
              <div className="choices">
                <div className="choices-head">
                  <span>Choices</span>
                  <div className="choices-actions">
                    <button
                      type="button"
                      className="btn ghost"
                      onClick={() => setQForm(f => ({ ...f, choices: [...f.choices, { label: "", isCorrect: false }] }))}
                      disabled={form.isPublished}
                    >
                      + Add choice
                    </button>
                    {qForm.choices.length > 2 && (
                      <button
                        type="button"
                        className="btn ghost"
                        onClick={() => setQForm(f => ({ ...f, choices: f.choices.slice(0, -1) }))}
                        disabled={form.isPublished}
                      >
                        − Remove last
                      </button>
                    )}
                  </div>
                </div>

                {qForm.choices.map((c, idx) => (
                  <div key={idx} className="choice-row">
                    <input
                      value={c.label}
                      placeholder={`Choice #${idx + 1}`}
                      onChange={e => {
                        const v = e.target.value;
                        setQForm(f => {
                          const copy = [...f.choices];
                          copy[idx] = { ...copy[idx], label: v };
                          return { ...f, choices: copy };
                        });
                      }}
                      disabled={form.isPublished}
                    />
                    <label className="inline">
                      <input
                        type="checkbox"
                        checked={c.isCorrect}
                        onChange={e => {
                          const checked = e.target.checked;
                          setQForm(f => {
                            let copy = [...f.choices];
                            if (f.type === QuestionType.Single && checked) {
                              copy = copy.map((x, i) => ({ ...x, isCorrect: i === idx }));
                            } else {
                              copy[idx] = { ...copy[idx], isCorrect: checked };
                            }
                            return { ...f, choices: copy };
                          });
                        }}
                        disabled={form.isPublished}
                      />
                      <span>Correct</span>
                    </label>
                  </div>
                ))}
              </div>
            )}

            {qForm.type === QuestionType.TrueFalse && (
              <div className="tf">
                <span>Is “True” the correct answer?</span>
                <div className="chip-group">
                  <button
                    type="button"
                    className={`chip ${qForm.isTrueCorrect === true ? "selected" : ""}`}
                    onClick={() => setQForm(f => ({ ...f, isTrueCorrect: true }))}
                    disabled={form.isPublished}
                  >
                    True
                  </button>
                  <button
                    type="button"
                    className={`chip ${qForm.isTrueCorrect === false ? "selected" : ""}`}
                    onClick={() => setQForm(f => ({ ...f, isTrueCorrect: false }))}
                    disabled={form.isPublished}
                  >
                    False
                  </button>
                </div>
              </div>
            )}

            {qForm.type === QuestionType.FillIn && (
              <label>
                <span>Expected answer</span>
                <input
                  value={qForm.textAnswer}
                  onChange={e => setQForm({ ...qForm, textAnswer: e.target.value })}
                  placeholder="Type the correct text…"
                  maxLength={2048}
                  disabled={form.isPublished}
                />
              </label>
            )}

            <div className="actions">
              <button
                type="button"
                className="btn primary"
                onClick={submitQuestion}
                disabled={!canCreateQuestion || qSaving || form.isPublished}
              >
                {editingId ? (qSaving ? "Saving…" : "Save changes") : (qSaving ? "Adding…" : "Add question")}
              </button>
              {editingId && (
                <button
                  type="button"
                  className="btn ghost"
                  onClick={resetQFormForCreate}
                  disabled={qSaving}
                >
                  Cancel
                </button>
              )}
            </div>
          </div>

          {questions.length > 0 && (
            <div className="q-list">
              <h4>Questions</h4>
              {questions.map(q => (
                <div className="q-item" key={q.id}>
                  <div className="q-title">
                    <strong>{q.question}</strong>
                    <span className="muted"> · {QuestionType[q.type]} · {q.points} pts</span>
                  </div>
                  {q.type === QuestionType.FillIn && q.textAnswer?.text && (
                    <div className="q-sub">Answer: <em>{q.textAnswer.text}</em></div>
                  )}
                  {(q.type === QuestionType.Single || q.type === QuestionType.Multi || q.type === QuestionType.TrueFalse) && q.choices?.length > 0 && (
                    <ul className="q-choices">
                      {q.choices.map(c => (
                        <li key={c.id ?? c.label}>
                          {c.label} {c.isCorrect ? "✓" : ""}
                        </li>
                      ))}
                    </ul>
                  )}
                  <div className="q-actions">
                    <button className="btn ghost" onClick={() => startEdit(q)} disabled={form.isPublished}>Edit</button>
                    <button className="btn danger" onClick={() => onDeleteQuestion(q.id)} disabled={form.isPublished}>Delete</button>
                  </div>
                </div>
              ))}
            </div>
          )}
        </section>
      )}
    </div>
  );
};

export default QuizEditorPage;

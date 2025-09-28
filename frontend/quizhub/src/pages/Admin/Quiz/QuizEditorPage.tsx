import React, { useEffect, useMemo, useState } from "react";
import "./QuizEditorPage.scss";
import { useNavigate, useParams } from "react-router-dom";
import { quizzesApi } from "../../../services/quizzes.api";
import type { Quiz } from "../../../models/quiz";
import { tagsApi } from "../../../services/tags.api";

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

  useEffect(() => {
    (async () => {
      setLoading(true); setErr(null);
      try {
        const tags = await tagsApi.list(0, 200);
        setAllTags(tags);
        if (!isCreate && quizId) {
          const q = await quizzesApi.getById(quizId);
          const initial: FormState = {
            name: q.name,
            description: q.description ?? "",
            difficultyLevel: q.difficultyLevel,
            timeInSeconds: q.timeInSeconds,
            isPublished: q.isPublished,
            tagIds: q.tags?.map(t => t.id) ?? [],
          };
          setForm(initial);
        } else {
          setForm(emptyForm);
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

  // Keep the page & form even if there's an error
  return (
    <div className="quiz-editor">
      <header className="qe-head">
        <h2>{isCreate ? "Create quiz" : "Edit quiz"}</h2>
      </header>

      {loading && <div className="loading">Loading…</div>}

      {/* Accessible inline error banner; stays in DOM so screen readers announce updates */}
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
    </div>
  );
};

export default QuizEditorPage;

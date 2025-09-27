import React, { useEffect, useMemo, useState } from "react";
import "./TagsPage.scss";
import { Tag } from "../../../models/tag";
import { tagsApi } from "../../../services/tags.api";

type UiTag = Tag & { _saving?: boolean; _editing?: boolean; _draftName?: string };

const PAGE_SIZE = 20;

const TagsPage: React.FC = () => {
  const [tags, setTags] = useState<UiTag[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [skip, setSkip] = useState(0);
  const [hasMore, setHasMore] = useState(true);

  const [newName, setNewName] = useState("");
  const [creating, setCreating] = useState(false);

  const load = async (reset = false) => {
        setLoading(true);
        setError(null);
        try {
            const nextSkip = reset ? 0 : skip;
            const data = await tagsApi.list(nextSkip, PAGE_SIZE);
            const arr = Array.isArray(data) ? data : []; // belt-and-suspenders; should already be array
            setHasMore(arr.length === PAGE_SIZE);
            setSkip(reset ? PAGE_SIZE : nextSkip + arr.length);
            setTags(reset ? arr : [...tags, ...arr]);
        } catch (e: any) {
            setError(e.message || "Failed to load tags.");
        } finally {
            setLoading(false);
        }
    };

  useEffect(() => {
    load(true);
  }, []);

  const onStartEdit = (id: string) => {
    setTags((prev) =>
      prev.map((t) => (t.id === id ? { ...t, _editing: true, _draftName: t.name } : t))
    );
  };

  const onCancelEdit = (id: string) => {
    setTags((prev) =>
      prev.map((t) => (t.id === id ? { ...t, _editing: false, _draftName: undefined } : t))
    );
  };

  const onDraftChange = (id: string, value: string) => {
    setTags((prev) =>
      prev.map((t) => (t.id === id ? { ...t, _draftName: value } : t))
    );
  };

  const onSaveEdit = async (id: string) => {
    const t = tags.find((x) => x.id === id);
    if (!t || !t._draftName || t._draftName.trim().length < 2) return;

    setTags((prev) => prev.map((x) => (x.id === id ? { ...x, _saving: true } : x)));
    try {
      const updated = await tagsApi.update(id, t._draftName.trim());
      setTags((prev) =>
        prev.map((x) =>
          x.id === id ? { ...updated, _editing: false, _saving: false } : x
        )
      );
    } catch (e: any) {
      alert(e.message || "Failed to update tag.");
      setTags((prev) => prev.map((x) => (x.id === id ? { ...x, _saving: false } : x)));
    }
  };

  const onDelete = async (id: string) => {
    const t = tags.find((x) => x.id === id);
    if (!t) return;
    if (!window.confirm(`Delete tag "${t.name}"?`)) return;

    const snapshot = [...tags];
    setTags((prev) => prev.filter((x) => x.id !== id));
    try {
      await tagsApi.delete(id);
    } catch (e: any) {
      alert(e.message || "Failed to delete tag.");
      setTags(snapshot);
    }
  };

  const onCreate = async (e: React.FormEvent) => {
    e.preventDefault();
    const name = newName.trim();
    if (name.length < 2) return;
    setCreating(true);
    try {
      const created = await tagsApi.create(name);
      setTags((prev) => [created, ...prev]);
      setNewName("");
    } catch (e: any) {
      alert(e.message || "Failed to create tag.");
    } finally {
      setCreating(false);
    }
  };

  const empty = useMemo(() => !loading && tags.length === 0, [loading, tags]);

  return (
    <div className="tags-page">
      <div className="tags-content">
        <section className="tags-table" aria-label="Tags">
          <div className="table-head">
            <div className="col name">Name</div>
            <div className="col actions" aria-hidden />
          </div>

          <div className="table-body" role="list">
            {tags.map((t) => (
              <div className="row" key={t.id} role="listitem" data-saving={t._saving ? "1" : undefined}>
                <div className="col name">
                  {!t._editing ? (
                    <span className="tag-name">{t.name}</span>
                  ) : (
                    <input
                      className="inline-input"
                      value={t._draftName ?? ""}
                      onChange={(e) => onDraftChange(t.id, e.target.value)}
                      maxLength={64}
                      disabled={t._saving}
                    />
                  )}
                </div>
                <div className="col actions">
                  {!t._editing ? (
                    <>
                      <button className="btn ghost" onClick={() => onStartEdit(t.id)} disabled={t._saving}>
                        Update
                      </button>
                      <button className="btn danger" onClick={() => onDelete(t.id)} disabled={t._saving}>
                        Delete
                      </button>
                    </>
                  ) : (
                    <>
                      <button className="btn primary" onClick={() => onSaveEdit(t.id)} disabled={t._saving}>
                        Save
                      </button>
                      <button className="btn ghost" onClick={() => onCancelEdit(t.id)} disabled={t._saving}>
                        Cancel
                      </button>
                    </>
                  )}
                </div>
              </div>
            ))}

            {empty && <div className="empty">No tags yet.</div>}
            {error && <div className="error">{error}</div>}
          </div>

          <div className="table-foot">
            <button className="btn more" onClick={() => load(false)} disabled={!hasMore || loading}>
              {loading ? "Loading..." : hasMore ? "Load more" : "No more"}
            </button>
          </div>
        </section>

        <aside className="tags-sidebar">
          <div className="card">
            <h3>Add new tag</h3>
            <form onSubmit={onCreate} className="add-form">
              <label>
                <span>Name</span>
                <input
                  value={newName}
                  onChange={(e) => setNewName(e.target.value)}
                  placeholder="e.g. JavaScript"
                  maxLength={64}
                  disabled={creating}
                />
              </label>
              <button className="btn primary block" type="submit" disabled={creating || newName.trim().length < 2}>
                {creating ? "Adding..." : "Add tag"}
              </button>
              <p className="muted">2–64 characters. Names must be unique.</p>
            </form>
          </div>
        </aside>
      </div>
    </div>
  );
};

export default TagsPage;

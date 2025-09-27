import React, { useRef, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { auth } from "../../services/auth.api";
import "./RegisterPage.scss";

type FormState = {
  username: string;
  email: string;
  password: string;
  confirmPassword: string;
  image: File | null;
};

const initialState: FormState = {
  username: "",
  email: "",
  password: "",
  confirmPassword: "",
  image: null,
};

const MAX_IMAGE_MB = 10;

const RegisterPage: React.FC = () => {
  const [form, setForm] = useState<FormState>(initialState);
  const [showPwd, setShowPwd] = useState(false);
  const [showPwd2, setShowPwd2] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);
  const [isDragging, setIsDragging] = useState(false);
  const inputRef = useRef<HTMLInputElement>(null);
  const navigate = useNavigate();

  const setFile = (file: File | null) => {
    setForm((s) => ({ ...s, image: file }));
  };

  const onPickClick = () => inputRef.current?.click();

  const onFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const f = e.target.files?.[0] ?? null;
    setFile(f);
  };

  const onDrop = (e: React.DragEvent<HTMLDivElement>) => {
    e.preventDefault();
    setIsDragging(false);
    const f = e.dataTransfer.files?.[0] ?? null;
    setFile(f);
  };

  const onDragOver = (e: React.DragEvent<HTMLDivElement>) => {
    e.preventDefault();
    setIsDragging(true);
  };

  const onDragLeave = (e: React.DragEvent<HTMLDivElement>) => {
    e.preventDefault();
    setIsDragging(false);
  };

  const onChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target as HTMLInputElement;
    setForm((s) => ({ ...s, [name]: value }));
  };

  const validate = () => {
    const u = form.username.trim();
    const em = form.email.trim();

    if (!u) return "Username is required.";
    if (u.length < 3 || u.length > 64) return "Username must be 3–64 characters.";
    if (!em) return "Email is required.";
    if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(em)) return "Email is not valid.";
    if (!form.password) return "Password is required.";
    if (form.password.length < 8) return "Password must be at least 8 characters.";
    if (!/[A-Z]/.test(form.password)) return "Password must contain an uppercase letter.";
    if (!/[a-z]/.test(form.password)) return "Password must contain a lowercase letter.";
    if (!/[0-9]/.test(form.password)) return "Password must contain a digit.";
    if (!/[^a-zA-Z0-9]/.test(form.password)) return "Password must contain a non-alphanumeric character.";
    if (form.password !== form.confirmPassword) return "Passwords do not match.";

    if (!form.image) return "Avatar image is required.";
    if (!form.image.type.startsWith("image/")) return "Avatar must be an image file.";
    if (form.image.size > MAX_IMAGE_MB * 1024 * 1024) return `Avatar must be ≤ ${MAX_IMAGE_MB}MB.`;

    return null;
  };

  const onSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    const message = validate();
    if (message) { setError(message); return; }

    setError(null);
    setLoading(true);
    try {
      await auth.register({
        username: form.username.trim(),
        email: form.email.trim(),
        password: form.password,
        image: form.image!,
      });
      navigate("/");
    } catch (err: any) {
      setError(err.message ?? "Registration failed.");
    } finally {
      setLoading(false);
    }
  };

  const clearImage = () => setFile(null);

  const previewUrl = form.image ? URL.createObjectURL(form.image) : null;

  return (
    <div className="auth-page">
      <div className="auth-card">
        <h1 className="auth-title">Create your account</h1>
        <p className="auth-subtitle">Join QuizApp to start playing</p>

        {error && <div className="auth-alert" role="alert">{error}</div>}

        <form onSubmit={onSubmit} className="auth-form">
          <label className="auth-label">
            <span>Username</span>
            <input
              className="auth-input"
              name="username"
              autoComplete="username"
              value={form.username}
              onChange={onChange}
              placeholder="yourname"
              disabled={loading}
            />
          </label>

          <label className="auth-label">
            <span>Email</span>
            <input
              className="auth-input"
              name="email"
              type="email"
              autoComplete="email"
              value={form.email}
              onChange={onChange}
              placeholder="you@example.com"
              disabled={loading}
            />
          </label>

          <label className="auth-label">
            <span>Password</span>
            <div className="auth-input-group">
              <input
                className="auth-input"
                type={showPwd ? "text" : "password"}
                name="password"
                autoComplete="new-password"
                value={form.password}
                onChange={onChange}
                placeholder="At least 8 chars, Aa1!"
                disabled={loading}
              />
              <button
                type="button"
                className="auth-eye"
                onClick={() => setShowPwd((s) => !s)}
                aria-label={showPwd ? "Hide password" : "Show password"}
                disabled={loading}
                title={showPwd ? "Hide password" : "Show password"}
              >
                {showPwd ? "Hide" : "Show"}
              </button>
            </div>
          </label>

          <label className="auth-label">
            <span>Confirm password</span>
            <div className="auth-input-group">
              <input
                className="auth-input"
                type={showPwd2 ? "text" : "password"}
                name="confirmPassword"
                autoComplete="new-password"
                value={form.confirmPassword}
                onChange={onChange}
                placeholder="Repeat password"
                disabled={loading}
              />
              <button
                type="button"
                className="auth-eye"
                onClick={() => setShowPwd2((s) => !s)}
                aria-label={showPwd2 ? "Hide password" : "Show password"}
                disabled={loading}
                title={showPwd2 ? "Hide password" : "Show password"}
              >
                {showPwd2 ? "Hide" : "Show"}
              </button>
            </div>
          </label>

          <div className="auth-label">
            <span>Avatar (required)</span>

            <div
              className={`uploader ${isDragging ? "is-dragging" : ""} ${previewUrl ? "has-file" : ""}`}
              onDragOver={onDragOver}
              onDragLeave={onDragLeave}
              onDrop={onDrop}
              role="button"
              tabIndex={0}
              onKeyDown={(e) => (e.key === "Enter" || e.key === " ") && onPickClick()}
              aria-label="Choose avatar image file"
              onClick={onPickClick}
            >
              {!previewUrl ? (
                <>
                  <div className="uploader-icon">🖼️</div>
                  <div className="uploader-text">
                    <strong>Click to upload</strong> or drag & drop
                  </div>
                  <div className="uploader-hint">PNG, JPG up to {MAX_IMAGE_MB}MB</div>
                </>
              ) : (
                <div className="uploader-preview">
                  <img src={previewUrl} alt="Selected avatar preview" />
                </div>
              )}
            </div>

            <input
              ref={inputRef}
              type="file"
              name="image"
              accept="image/*"
              className="uploader-input"
              onChange={onFileChange}
              disabled={loading}
              aria-hidden="true"
              tabIndex={-1}
            />

            {form.image && (
              <div className="auth-file-row">
                <span className="auth-file-name" title={form.image.name}>{form.image.name}</span>
                <button type="button" className="auth-file-clear" onClick={clearImage} disabled={loading}>
                  Remove
                </button>
              </div>
            )}
          </div>

          <button className="auth-btn" type="submit" disabled={loading}>
            {loading ? "Creating account..." : "Sign up"}
          </button>
        </form>

        <div className="auth-foot">
          <span>Already have an account?</span>{" "}
          <Link to="/login" className="auth-link">Sign in</Link>
        </div>
      </div>
    </div>
  );
};

export default RegisterPage;
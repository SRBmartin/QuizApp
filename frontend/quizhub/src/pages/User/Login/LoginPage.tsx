import React, { useState } from "react";
import { useNavigate, Link } from "react-router-dom";
import { auth } from "../../../services/auth.api";
import "./LoginPage.scss"
import { useAuth } from "../../../context/AuthContext";

type FormState = {
  usernameOrEmail: string;
  password: string;
};

const initialState: FormState = { usernameOrEmail: "", password: "" };

const LoginPage: React.FC = () => {
  const [form, setForm] = useState<FormState>(initialState);
  const [showPwd, setShowPwd] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();

  const { login } = useAuth();

  const onChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setForm((s) => ({ ...s, [e.target.name]: e.target.value }));
  };

  const validate = () => {
    if (!form.usernameOrEmail.trim()) return "Username or email is required.";
    if (!form.password) return "Password is required.";
    return null;
  };

  const onSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    const message = validate();
    if (message) { setError(message); return; }

    setError(null);
    setLoading(true);
    try {
      const dto = await auth.login({
        usernameOrEmail: form.usernameOrEmail.trim(),
        password: form.password,
      });
      login(dto.accessToken);
      navigate("/");
    } catch (err: any) {
      setError(err.message ?? "Login failed.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="auth-page">
      <div className="auth-card">
        <h1 className="auth-title">Welcome back</h1>
        <p className="auth-subtitle">Sign in to continue</p>

        {error && <div className="auth-alert" role="alert">{error}</div>}

        <form onSubmit={onSubmit} className="auth-form">
          <label className="auth-label">
            <span>Username or Email</span>
            <input
              className="auth-input"
              name="usernameOrEmail"
              autoComplete="username"
              value={form.usernameOrEmail}
              onChange={onChange}
              placeholder="yourname or your@email.com"
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
                autoComplete="current-password"
                value={form.password}
                onChange={onChange}
                placeholder="••••••••"
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
                {showPwd ? "🙈" : "👁️"}
              </button>
            </div>
          </label>

          <button className="auth-btn" type="submit" disabled={loading}>
            {loading ? "Signing in..." : "Sign in"}
          </button>
        </form>

        <div className="auth-foot">
          <span>Don’t have an account?</span>{" "}
          <Link to="/register" className="auth-link">Register</Link>
        </div>
      </div>
    </div>
  );
};

export default LoginPage;

import React from "react";
import { Link, NavLink, useNavigate } from "react-router-dom";
import "./Header.scss";
import { useAuth } from "../../../context/AuthContext";

const Header: React.FC = () => {
  const { isAuthenticated, isAdmin, logout } = useAuth();
  const navigate = useNavigate();

  const onLogout = () => {
    const ok = window.confirm("Are you sure you want to log out?");
    if (!ok) return;

    logout();
    navigate("/login", { replace: true });
  };

  return (
    <header className="app-header">
      <div className="header-content container">
        <Link to="/" className="logo">
          <img src="/assets/quiz/quiz-icon.svg" alt="QuizHub logo" className="logo-icon" />
          <span className="logo-text">QuizHub</span>
        </Link>

        <nav className="nav-links">
          <NavLink to="/" end className={({ isActive }) => (isActive ? "active" : "")}>
            Home
          </NavLink>
          <NavLink to="/quizzes" className={({ isActive }) => (isActive ? "active" : "")}>
            Quizzes
          </NavLink>
          <NavLink to="/leaderboard" className={({ isActive }) => (isActive ? "active" : "")}>
            Leaderboard
          </NavLink>
          
          {isAdmin && (
              <NavLink to="/admin/tags" className={({ isActive }) => (isActive ? "active" : "")}>
              Tags
            </NavLink>
          )}

          {isAdmin && (
            <NavLink to="/admin/quizzes" className={({ isActive }) => (isActive ? "active" : "")}>
              Admin Quizzes
            </NavLink>
          )}

        </nav>

        <div className="header-actions">
          {!isAuthenticated ? (
            <>
              <NavLink to="/login" className="btn btn-outline">Login</NavLink>
              <NavLink to="/register" className="btn btn-primary">Sign Up</NavLink>
            </>
          ) : (
            <button type="button" onClick={onLogout} className="btn btn-outline">
              Logout
            </button>
          )}
        </div>
      </div>
    </header>
  );
};

export default Header;
import React from "react";
import { Link, NavLink } from "react-router-dom";
import "./Header.scss";

const Header: React.FC = () => {
  return (
    <header className="app-header">
      <div className="header-content container">
        <Link to="/" className="logo">
          <img src="/assets/quiz/quiz-icon.svg" alt="QuizHub logo" className="logo-icon" />
          <span className="logo-text">QuizHub</span>
        </Link>
        <nav className="nav-links">
          <NavLink to="/" end className={({ isActive }) => isActive ? "active" : ""}>
            Home
          </NavLink>
          <NavLink to="/quizzes" className={({ isActive }) => isActive ? "active" : ""}>
            Quizzes
          </NavLink>
          <NavLink to="/leaderboard" className={({ isActive }) => isActive ? "active" : ""}>
            Leaderboard
          </NavLink>
        </nav>
        <div className="header-actions">
          <NavLink to="/login" className="btn btn-outline">
            Login
          </NavLink>
          <NavLink to="/register" className="btn btn-primary">
            Sign Up
          </NavLink>
        </div>
      </div>
    </header>
  );
};

export default Header;
import React from "react";
import { Link } from "react-router-dom";
import "./Footer.scss";

const Footer: React.FC = () => {
  const currentYear = new Date().getFullYear();
  return (
    <footer className="app-footer">
      <div className="footer-content container">
        <div className="footer-brand">
          <img src="/assets/quiz/quiz-icon.svg" alt="QuizHub logo" className="footer-logo-icon" />
          <span className="footer-logo-text">QuizHub</span>
        </div>
        <div className="footer-links">
          <div className="column">
            <h4>Company</h4>
            <Link to="/about">About</Link>
            <Link to="/contact">Contact</Link>
          </div>
          <div className="column">
            <h4>Legal</h4>
            <Link to="/terms">Terms of Use</Link>
            <Link to="/privacy">Privacy Policy</Link>
          </div>
        </div>
        <div className="footer-bottom">
          <span>© {currentYear} QuizHub. All rights reserved.</span>
        </div>
      </div>
    </footer>
  );
};

export default Footer;
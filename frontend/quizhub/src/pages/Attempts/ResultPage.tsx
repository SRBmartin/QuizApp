import React from "react";
import "./ResultPage.scss";
import { useParams, Link } from "react-router-dom";

const ResultPage: React.FC = () => {
  const { attemptId } = useParams<{ attemptId: string }>();
  return (
    <div className="result-page">
      <header className="rp-head">
        <h2 className="rp-title">Results</h2>
      </header>

      <div className="placeholder">
        <p>Your results will appear here.</p>
        <p>Attempt ID: <strong>{attemptId}</strong></p>
        <Link to="/quizzes" className="btn ghost">Back to quizzes</Link>
      </div>
    </div>
  );
};

export default ResultPage;

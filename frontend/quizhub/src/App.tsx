import React from "react";
import { BrowserRouter as Router, Routes, Route, Navigate } from "react-router-dom";
import "./App.scss";

import Header from "./components/common/Header/Header";
import Footer from "./components/common/Footer/Footer";

import LoginPage from "./pages/User/Login/LoginPage";
import RegisterPage from "./pages/User/Register/RegisterPage";

import { AuthProvider } from "./context/AuthContext";
import RequireAuth from "./routes/RequireAuth";
import RequireGuest from "./routes/RequireGuest";
import TagsPage from "./pages/Admin/Tag/TagsPage";
import RequireAdmin from "./routes/RequireAdmin";
import AdminQuizzesPage from "./pages/Admin/Quiz/AdminQuizzesPage";
import QuizEditorPage from "./pages/Admin/Quiz/QuizEditorPage";
import QuizzesPage from "./pages/Quizzes/QuizzesPage";
import QuizPage from "./pages/Quizzes/QuizPage";
import AnswerQuizPage from "./pages/Attempts/AnswerQuizPage";
import ResultPage from "./pages/Attempts/ResultPage";
import HomePage from "./pages/Home/HomePage";

const Home: React.FC = () => (
  <div style={{ padding: "2rem" }}>
    <h2>Home</h2>
    <p>Welcome to QuizApp.</p>
  </div>
);

function App() {
  return (
    <AuthProvider>
      <Router>
        <div className="app-wrapper">
          <Header />
          <main className="main-content">
            <Routes>
              {/* Guest-only routes */}
              <Route element={<RequireGuest redirectTo="/" />}>
                <Route path="/login" element={<LoginPage />} />
                <Route path="/register" element={<RegisterPage />} />
              </Route>

              {/* Auth-only routes */}
              <Route element={<RequireAuth />}>
                <Route path="/" element={<HomePage />} />
                <Route path="/quizzes" element={<QuizzesPage />} />
                <Route path="/quiz/:id" element={<QuizPage />} />
                <Route path="/quiz/:quizId/answer" element={<AnswerQuizPage />} />
                <Route path="/attempt/:attemptId/result" element={<ResultPage />} />

                <Route element={<RequireAdmin />}>
                  <Route path="/admin/tags" element={<TagsPage />} />
                  <Route path="/admin/quizzes" element={<AdminQuizzesPage />} />
                  <Route path="/admin/quizzes/new" element={<QuizEditorPage />} />
                  <Route path="/admin/quizzes/:quizId" element={<QuizEditorPage />} />
                </Route>
              </Route>

              <Route path="*" element={<Navigate to="/" />} />
            </Routes>
          </main>
          <Footer />
        </div>
      </Router>
    </AuthProvider>
  );
}

export default App;
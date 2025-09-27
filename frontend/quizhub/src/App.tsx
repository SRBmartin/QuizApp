import React from "react";
import { BrowserRouter as Router, Routes, Route, Navigate } from "react-router-dom";
import "./App.scss";

import Header from "./components/common/Header/Header";
import Footer from "./components/common/Footer/Footer";

import LoginPage from "./pages/User/Login/LoginPage";
import RegisterPage from "./pages/Register/RegisterPage";

const Home: React.FC = () => (
  <div style={{ padding: "2rem" }}>
    <h2>Home</h2>
    <p>Welcome to QuizApp.</p>
  </div>
);

function App() {
  return (
    <Router>
      <div className="app-wrapper">
        <Header />
        <main className="main-content">
          <Routes>
            <Route path="/" element={<Home />} />
            <Route path="/login" element={<LoginPage />} />
            <Route path="/register" element={<RegisterPage />} />
            <Route path="*" element={<Navigate to="/" />} />
          </Routes>
        </main>
        <Footer />
      </div>
    </Router>
  );
}

export default App;

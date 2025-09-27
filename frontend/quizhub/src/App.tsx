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
              <Route path="/" element={<Home />} />

              {/* Guest-only routes */}
              <Route element={<RequireGuest redirectTo="/" />}>
                <Route path="/login" element={<LoginPage />} />
                <Route path="/register" element={<RegisterPage />} />
              </Route>

              {/* Auth-only routes */}
              <Route element={<RequireAuth />}>
                <Route element={<RequireAdmin />}>
                  <Route path="/admin/tags" element={<TagsPage />} />
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
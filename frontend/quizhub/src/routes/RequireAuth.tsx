import React from "react";
import { Navigate, Outlet, useLocation } from "react-router-dom";
import { useAuth } from "../context/AuthContext";

const RequireAuth: React.FC = () => {
  const { isAuthenticated } = useAuth();
  const loc = useLocation();
  return isAuthenticated ? <Outlet /> : <Navigate to="/login" replace state={{ from: loc }} />;
};

export default RequireAuth;

import React from "react";
import { Navigate, Outlet } from "react-router-dom";
import { useAuth } from "../context/AuthContext";

const RequireAdmin: React.FC = () => {
  const { isAuthenticated, isAdmin } = useAuth();
  if (!isAuthenticated || !isAdmin) return <Navigate to="/" replace />;
  return <Outlet />;
};

export default RequireAdmin;

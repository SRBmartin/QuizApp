// src/routes/RequireGuest.tsx
import React from "react";
import { Navigate, Outlet } from "react-router-dom";
import { useAuth } from "../context/AuthContext";

const RequireGuest: React.FC<{ redirectTo?: string }> = ({ redirectTo = "/" }) => {
  const { isAuthenticated } = useAuth();
  return isAuthenticated ? <Navigate to={redirectTo} replace /> : <Outlet />;
};

export default RequireGuest;

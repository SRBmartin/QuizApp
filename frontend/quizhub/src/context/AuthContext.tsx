import React, { createContext, useContext, useEffect, useMemo, useState } from "react";
import { auth } from "../services/auth.api";
import { hasRole } from "../services/jwt.util";
import type { AuthUser, AuthState, JwtRole } from "../models/auth";

type AuthContextShape = AuthState & {
  refresh(): void;
  hasRole(role: JwtRole): boolean;
  login(token: string): void;
  logout(): void;
};

const AuthContext = createContext<AuthContextShape | undefined>(undefined);

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const compute = (): { user: AuthUser | null } => ({ user: auth.getCurrentUser() });

  const [{ user }, setState] = useState(compute);

  const refresh = () => setState(compute);
  useEffect(() => {
    const handler = (e: StorageEvent) => {
      if (e.key === "quizhub.token") refresh();
    };
    window.addEventListener("storage", handler);
    return () => window.removeEventListener("storage", handler);
  }, []);

  const isAuthenticated = !!user;
  const isAdmin = hasRole(user, "Admin");

  const value = useMemo<AuthContextShape>(() => ({
    isAuthenticated,
    user,
    isAdmin,
    refresh,
    hasRole: (r) => hasRole(user, r),
    login: (token: string) => { auth.loginSuccess(token); refresh(); },
    logout: () => { auth.logout(); refresh(); },
  }), [isAuthenticated, isAdmin, user]);

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};

export const useAuth = () => {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error("useAuth must be used within <AuthProvider>");
  return ctx;
};

import React, { createContext, useContext, useEffect, useMemo, useState } from "react";
import { authStorage } from "../services/auth.storage";

type AuthCtx = {
  token: string | null;
  isAuthenticated: boolean;
};

const Ctx = createContext<AuthCtx>({ token: null, isAuthenticated: false });

export const AuthProvider: React.FC<React.PropsWithChildren> = ({ children }) => {
  const [token, setToken] = useState<string | null>(() => authStorage.token);

  useEffect(() => {
    return authStorage.onChange(() => setToken(authStorage.token));
  }, []);

  const value = useMemo<AuthCtx>(() => ({
    token,
    isAuthenticated: !!token
  }), [token]);

  return <Ctx.Provider value={value}>{children}</Ctx.Provider>;
};

export const useAuth = () => useContext(Ctx);
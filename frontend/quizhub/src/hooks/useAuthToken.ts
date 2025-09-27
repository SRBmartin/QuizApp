import { useMemo } from "react";

export function useAuthToken() {
  const token = useMemo(() => localStorage.getItem("auth.token"), []);
  return token;
}

import { jwtDecode } from "jwt-decode";
import type { JwtClaims, AuthUser, JwtRole } from "../models/auth";

export function decodeClaims(token: string | null): JwtClaims | null {
  if (!token) return null;
  try {
    return jwtDecode<JwtClaims>(token);
  } catch {
    return null;
  }
}

export function isExpired(claims: JwtClaims | null): boolean {
  if (!claims?.exp) return false;
  const nowSec = Math.floor(Date.now() / 1000);
  return claims.exp <= nowSec;
}

export function toAuthUser(claims: JwtClaims | null): AuthUser | null {
  if (!claims || !claims.sub || !claims.username) return null;

  const raw = claims.role;
  const roles: JwtRole[] = Array.isArray(raw)
    ? (raw as JwtRole[])
    : raw
      ? [raw as JwtRole]
      : [];

  return {
    id: claims.sub,
    username: claims.username,
    roles,
  };
}

export function hasRole(user: AuthUser | null, role: JwtRole): boolean {
  return !!user?.roles?.includes(role);
}

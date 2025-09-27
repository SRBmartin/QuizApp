export type JwtRole = "Admin" | "User";

export interface JwtClaims {
  sub: string;
  username: string;
  role?: JwtRole | JwtRole[];
  jti?: string;
  iat?: number;
  nbf?: number;
  exp?: number;
  iss?: string;
  aud?: string;
}

export interface AuthUser {
  id: string;
  username: string;
  roles: JwtRole[];
}

export interface AuthState {
  isAuthenticated: boolean;
  user: AuthUser | null;
  isAdmin: boolean;
}

import { AuthDto, LoginRequest, RegisterForm } from "../types/User";
import { http } from "./http";
import { authStorage, TOKEN_KEY } from "./auth.storage";
import { decodeClaims, isExpired, toAuthUser } from "./jwt.util";
import { AuthUser } from "../models/auth";

export const auth = {
  get token() {
    return authStorage.token;
  },

  async login(req: LoginRequest): Promise<AuthDto> {
    const dto = await http<AuthDto>("/api/Users/login", {
      method: "POST",
      body: JSON.stringify(req),
    });
    authStorage.token = dto.accessToken;
    return dto;
  },

  async register(form: RegisterForm): Promise<AuthDto> {
    const fd = new FormData();
    fd.set("username", form.username);
    fd.set("email", form.email);
    fd.set("password", form.password);
    if (form.image) fd.set("image", form.image);

    const dto = await http<AuthDto>("/api/Users/register", {
      method: "POST",
      body: fd,
    });
    authStorage.token = dto.accessToken;
    return dto;
  },

  logout() {
    authStorage.clear();
  },

  getToken(): string | null {
    return localStorage.getItem(TOKEN_KEY);
  },

  getCurrentUser(): AuthUser | null {
    const claims = decodeClaims(this.getToken());
    if (!claims || isExpired(claims)) return null;
    return toAuthUser(claims);
  },

  loginSuccess(token: string) {
    localStorage.setItem(TOKEN_KEY, token);
  },

};

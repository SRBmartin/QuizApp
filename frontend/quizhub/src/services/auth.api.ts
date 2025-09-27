import { AuthDto, LoginRequest, RegisterForm } from "../types/User";
import { http } from "./http";

const TOKEN_KEY = "auth.token";

export const auth = {
  get token() {
    return localStorage.getItem(TOKEN_KEY);
  },

  set token(value: string | null) {
    if (!value) localStorage.removeItem(TOKEN_KEY);
    else localStorage.setItem(TOKEN_KEY, value);
  },

  async login(req: LoginRequest): Promise<AuthDto> {
    const dto = await http<AuthDto>("/api/Users/login", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(req),
    });
    this.token = dto.accessToken;
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
    this.token = dto.accessToken;
    return dto;
  },

  logout() {
    this.token = null;
  },
};

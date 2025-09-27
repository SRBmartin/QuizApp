export type LoginRequest = {
  usernameOrEmail: string;
  password: string;
};

export type RegisterForm = {
  username: string;
  email: string;
  password: string;
  image?: File | null;
};

export type AuthDto = {
  accessToken: string;
};
import { authStorage } from "./auth.storage";
import { API_BASE } from "../config";

export async function http<T>(path: string, init?: RequestInit): Promise<T> {
  const url = `${API_BASE}${path}`;
  const headers = new Headers(init?.headers ?? {});
  const body = init?.body;

  const token = authStorage.token;
  if (token) {
    headers.set("Authorization", `Bearer ${token}`);
  }

  const isFormData = typeof FormData !== "undefined" && body instanceof FormData;
  if (!isFormData && body && !headers.has("Content-Type")) {
    headers.set("Content-Type", "application/json");
  }

  const res = await fetch(url, { ...init, headers });

  const isJson = res.headers.get("content-type")?.includes("application/json");
  const payload = isJson ? await res.json() : null;

  if (!res.ok) {
    if (res.status === 401) {
      authStorage.clear();
    }
    const message =
      (payload?.message as string) ??
      (payload?.error as string) ??
      `Request failed with status ${res.status}`;
    throw new Error(message);
  }

  return payload as T;
}

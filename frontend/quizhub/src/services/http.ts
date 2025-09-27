import { API_BASE } from "../config";

export async function http<T>(
  path: string,
  init?: RequestInit
): Promise<T> {
  const res = await fetch(`${API_BASE}${path}`, {
    headers: {
      ...(init?.headers ?? {}),
    },
    ...init,
  });

  const isJson = res.headers.get("content-type")?.includes("application/json");
  const payload = isJson ? await res.json() : null;

  if (!res.ok) {
    const message =
      (payload?.message as string) ??
      (payload?.error as string) ??
      `Request failed with status ${res.status}`;
    throw new Error(message);
  }

  return payload as T;
}

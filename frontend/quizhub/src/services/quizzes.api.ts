import { http } from "./http";
import type { Paged } from "../models/pages";
import type { Quiz } from "../models/quiz";

function qs(params: Record<string, string | number | undefined>) {
  const sp = new URLSearchParams();
  for (const [k, v] of Object.entries(params)) if (v !== undefined) sp.set(k, String(v));
  const s = sp.toString();
  return s ? `?${s}` : "";
}

export const quizzesApi = {
  async list(skip?: number, take?: number): Promise<Paged<Quiz>> {
    const query = qs({ skip, take });
    return http<Paged<Quiz>>(`/api/Quiz${query}`, { method: "GET" });
  },

  getById(id: string): Promise<Quiz> {
    return http<Quiz>(`/api/Quiz/${id}`, { method: "GET" });
  },

  create(body: { name: string; description?: string | null; difficultyLevel: number; timeInSeconds: number; }): Promise<Quiz> {
    return http<Quiz>("/api/Quiz", { method: "POST", body: JSON.stringify(body) });
  },

  update(id: string, body: { name: string; description?: string | null; difficultyLevel: number; timeInSeconds: number; isPublished: boolean; }): Promise<Quiz> {
    return http<Quiz>(`/api/Quiz/${id}`, { method: "PUT", body: JSON.stringify(body) });
  },

  delete(id: string): Promise<void> {
    return http<void>(`/api/Quiz/${id}`, { method: "DELETE" });
  },

  updateTags(quizId: string, tagIds: string[]): Promise<{ id: string; name: string }[]> {
    return http<{ id: string; name: string }[]>(`/api/Quiz/${quizId}/tags`, {
      method: "PUT",
      body: JSON.stringify({ tagIds }),
    });
  },
};

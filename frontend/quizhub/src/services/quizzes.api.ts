import { http } from "./http";
import type { LeaderboardResponse, Quiz } from "../models/quiz";

function qs(params: Record<string, string | number | undefined>) {
  const sp = new URLSearchParams();
  for (const [k, v] of Object.entries(params)) {
    if (v !== undefined && v !== null) sp.set(k, String(v));
  }
  const s = sp.toString();
  return s ? `?${s}` : "";
}

type Paged<T> = {
  items: T[];
  total: number;
  skip: number;
  take: number;
};

export const quizzesApi = {
  async list(
    skip: number,
    take: number,
    opts?: { tagId?: string | null; difficulty?: number | null; q?: string | null }
  ): Promise<Paged<Quiz>> {
    const query = qs({
      skip,
      take,
      tagId: opts?.tagId || undefined,
      difficulty:
        opts?.difficulty !== undefined && opts?.difficulty !== null
          ? opts.difficulty
          : undefined,
      q: opts?.q?.trim() ? opts.q.trim() : undefined,
    });

    return http<Paged<Quiz>>(`/api/Quiz${query}`, { method: "GET" });
  },

  async getById(id: string): Promise<Quiz> {
    return http<Quiz>(`/api/Quiz/${id}`, { method: "GET" });
  },

  async create(body: {
    name: string;
    description?: string;
    difficultyLevel: number;
    timeInSeconds: number;
  }): Promise<Quiz> {
    return http<Quiz>("/api/Quiz", {
      method: "POST",
      body: JSON.stringify(body),
    });
  },

  async update(id: string, body: {
    name: string;
    description?: string;
    difficultyLevel: number;
    timeInSeconds: number;
    isPublished: boolean;
  }): Promise<Quiz> {
    return http<Quiz>(`/api/Quiz/${id}`, {
      method: "PUT",
      body: JSON.stringify(body),
    });
  },

  async delete(id: string): Promise<void> {
    return http<void>(`/api/Quiz/${id}`, { method: "DELETE" });
  },

  async updateTags(quizId: string, tagIds: string[]): Promise<void> {
    return http<void>(`/api/Quiz/${quizId}/tags`, {
      method: "PUT",
      body: JSON.stringify({ tagIds }),
    });
  },

  async getQuizTop(quizId: string, opts?: { period?: "all" | "month" | "week"; take?: number }): Promise<LeaderboardResponse> {
    const sp = new URLSearchParams();
    sp.set("period", (opts?.period ?? "all"));
    sp.set("take", String(opts?.take ?? 20));
    return http<LeaderboardResponse>(`/api/Quiz/quizzes/${quizId}/top?${sp.toString()}`, { method: "GET" });
  }
};

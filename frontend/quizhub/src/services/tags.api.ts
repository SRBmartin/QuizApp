import { Tag } from "../models/tag";
import { http } from "./http";

function qs(params: Record<string, string | number | undefined>) {
  const sp = new URLSearchParams();
  for (const [k, v] of Object.entries(params)) {
    if (v !== undefined && v !== null) sp.set(k, String(v));
  }
  const s = sp.toString();
  return s ? `?${s}` : "";
}

export const tagsApi = {
  async list(skip?: number, take?: number): Promise<Tag[]> {
    const query = qs({ skip, take });
    
    const data = await http<Tag[] | null>(`/api/Tag${query}`, { method: "GET" });
    
    return Array.isArray(data) ? data : [];
  },

  create(name: string): Promise<Tag> {
    return http<Tag>("/api/Tag", {
      method: "POST",
      body: JSON.stringify({ name }),
    });
  },

  update(id: string, name: string): Promise<Tag> {
    return http<Tag>(`/api/Tag/${id}`, {
      method: "PUT",
      body: JSON.stringify({ name }),
    });
  },

  delete(id: string): Promise<void> {
    return http<void>(`/api/Tag/${id}`, { method: "DELETE" });
  },
};

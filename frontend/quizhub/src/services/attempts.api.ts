import { http } from "./http";
import type { AttemptQuestion, AttemptState, SaveAnswerRequest, StartAttemptResponse } from "../models/attempt";
import type { AttemptResultSummary, AttemptResultReview, AttemptResultCombined } from "../models/attempt-result";
import type { MyAttempt } from "../models/my-attempt";
import { Paged } from "../models/pages";

export type NextResponse = AttemptQuestion | StartAttemptResponse;

export const attemptsApi = {
  start(quizId: string): Promise<StartAttemptResponse> {
    return http<StartAttemptResponse>(`/api/Attempt/quizzes/${quizId}/attempts`, { method: "POST" });
  },
  getState(attemptId: string): Promise<AttemptState> {
    return http<AttemptState>(`/api/Attempt/attempts/${attemptId}`);
  },
  next(attemptId: string): Promise<NextResponse> {
    return http<NextResponse>(`/api/Attempt/attempts/${attemptId}/next`);
  },
  saveAnswer(attemptId: string, questionId: string, body: SaveAnswerRequest): Promise<AttemptQuestion> {
    return http<AttemptQuestion>(`/api/Attempt/attempts/${attemptId}/questions/${questionId}/answer`, {
      method: "PUT",
      body: JSON.stringify(body),
    });
  },
  submit(attemptId: string): Promise<AttemptState> {
    return http<AttemptState>(`/api/Attempt/attempts/${attemptId}/submit`, { method: "POST" });
  },

  resultSummary(attemptId: string): Promise<AttemptResultSummary> {
    return http<AttemptResultSummary>(`/api/Attempt/attempts/${attemptId}/result/summary`, { method: "GET" });
  },
  resultReview(attemptId: string): Promise<AttemptResultReview> {
    return http<AttemptResultReview>(`/api/Attempt/attempts/${attemptId}/result/review`, { method: "GET" });
  },
  resultCombined(attemptId: string): Promise<AttemptResultCombined> {
    return http<AttemptResultCombined>(`/api/Attempt/attempts/${attemptId}/result`, { method: "GET" });
  },

  myAttempts(skip: number, take: number, opts?: { status?: "InProgress" | "Completed" | number }): Promise<Paged<MyAttempt>> {
    const sp = new URLSearchParams();
    sp.set("skip", String(skip));
    sp.set("take", String(take));
    if (opts?.status !== undefined && opts?.status !== null) sp.set("status", String(opts.status));
    const q = sp.toString() ? `?${sp.toString()}` : "";
    return http<Paged<MyAttempt>>(`/api/Attempt/my${q}`, { method: "GET" });
  },
  
};

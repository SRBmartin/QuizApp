import { http } from "./http";
import type { AttemptQuestion, AttemptState, SaveAnswerRequest, StartAttemptResponse } from "../models/attempt";
import type { AttemptResultSummary, AttemptResultReview, AttemptResultCombined } from "../models/attempt-result";

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

  // ---------- Results ----------
  resultSummary(attemptId: string): Promise<AttemptResultSummary> {
    return http<AttemptResultSummary>(`/api/Attempt/attempts/${attemptId}/result/summary`, { method: "GET" });
  },
  resultReview(attemptId: string): Promise<AttemptResultReview> {
    return http<AttemptResultReview>(`/api/Attempt/attempts/${attemptId}/result/review`, { method: "GET" });
  },

  // ✅ Provide a real implementation (or remove usage in the UI)
  resultCombined(attemptId: string): Promise<AttemptResultCombined> {
    return http<AttemptResultCombined>(`/api/Attempt/attempts/${attemptId}/result`, { method: "GET" });
  },
};

import { http } from "./http";
import { QuestionType, Question } from "../models/question";

export const questionsApi = {
  create(quizId: string, body: {
    question: string;
    points: number;
    questionType: QuestionType;
    choices?: { label: string; isCorrect: boolean }[];
    isTrueCorrect?: boolean | null;
    textAnswer?: string | null;
  }): Promise<Question> {
    return http<Question>(`/api/Question/${quizId}`, {
      method: "POST",
      body: JSON.stringify(body),
    });
  },

  update(questionId: string, body: {
    question: string;
    points: number;
    choices?: { id?: string; label: string; isCorrect: boolean }[];
    isTrueCorrect?: boolean | null;
    textAnswer?: string | null;
  }): Promise<Question> {
    return http<Question>(`/api/Question/${questionId}`, {
      method: "PUT",
      body: JSON.stringify(body),
    });
  },

  delete(questionId: string): Promise<void> {
    return http<void>(`/api/Question/${questionId}`, { method: "DELETE" });
  },
};

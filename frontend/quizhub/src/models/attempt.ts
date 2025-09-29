import { QuestionType } from "./question";

export type AttemptQuestionOption = {
  id: string;
  text: string;
};

export type AttemptQuestion = {
  id: string;
  quizId: string;
  type: QuestionType;
  question: string;
  points: number;
  options: AttemptQuestionOption[];
  isLast: boolean;
};

export type AttemptState = {
  id: string;
  quizId: string;
  status: "InProgress" | "Completed" | number;
  startedAt: string;
  submittedAt?: string | null;
  totalScore: number;
  timeLimitSeconds: number;
  answeredCount: number;
  totalQuestions: number;
  timeLeftSeconds: number;
};

export type SaveAnswerRequest = {
  selectedChoiceIds?: string[];
  submittedText?: string | null;
};

export type StartAttemptResponse = {
  attemptId: string;
  question: AttemptQuestion;
  timeLeftSeconds: number;
  timeLimitSeconds: number;
};

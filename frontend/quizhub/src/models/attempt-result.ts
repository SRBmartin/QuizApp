import { QuestionType } from "./question";

export type AttemptResultSummary = {
  attemptId: string;
  quizId: string;
  quizName?: string;
  status: "Completed" | "InProgress" | number;
  totalQuestions: number;
  correctAnswers: number;
  totalScore: number;
  maxScore: number;
  percentage: number;
  timeLimitSeconds: number;
  durationSeconds?: number;
  startedAt?: string;
  submittedAt?: string;
};

export type AttemptReviewItemOption = {
  id: string;
  text: string;
  isCorrect?: boolean;
  selectedByUser?: boolean;
};

export type AttemptReviewItem = {
  questionId: string;
  question: string;
  type: QuestionType | number;
  points: number;
  awardedScore: number;
  isCorrect: boolean;
  options?: AttemptReviewItemOption[];

  textAnswer?: {
    submittedText?: string | null;
    expectedText?: string | null;
  };
};

export type AttemptResultReview = {
  attemptId: string;
  items: AttemptReviewItem[];
};

export type AttemptResultCombined = {
  summary: AttemptResultSummary;
  items: AttemptReviewItem[];
};

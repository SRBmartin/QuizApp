export type MyAttempt = {
  attemptId: string;
  quizId: string;
  quizName: string;

  status: "InProgress" | "Completed" | number;
  startedAt: string;
  submittedAt?: string | null;

  totalScore: number;
  maxScore: number;
  percentage: number;

  durationSeconds?: number | null;
};

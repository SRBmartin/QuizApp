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

export type AttemptUser = {
  id: string;
  username: string;
  photo?: string | null;
};

export type QuizAttemptRow = {
  attemptId: string;
  quizId: string;
  quizName: string;

  user: AttemptUser;

  status: "InProgress" | "Completed" | number;
  startedAt: string;
  submittedAt?: string | null;

  totalScore: number;
  maxScore: number;
  percentage: number;

  durationSeconds?: number | null;
};
import { Tag } from "./tag";

export interface QuestionChoice {
  id: string;
  label: string;
  isCorrect: boolean;
}

export interface QuestionText {
  id: string;
  text: string;
}

export interface Question {
  id: string;
  quizId: string;
  points: number;
  type: number;              // 0 Single, 1 Multi, 2 TrueFalse, 3 FillIn
  question: string;
  createdAt: string;
  isDeleted: boolean;
  choices: QuestionChoice[];
  textAnswer?: QuestionText | null;
}

export type Quiz = {
  id: string;
  name: string;
  description?: string | null;
  difficultyLevel: number;
  timeInSeconds: number;
  createdAt: string;
  isPublished: boolean;
  isDeleted: boolean;
  questionCount?: number;
  questions: Question[];
  tags: Tag[];
};

export type LeaderboardUser = {
  id: string;
  username: string;
  photo?: string | null;
};

export type LeaderboardEntry = {
  rank: number;
  user: LeaderboardUser;
  totalScore: number;
  percentage: number;
  submittedAt?: string | null;
};

export type LeaderboardResponse = {
  quizId: string;
  quizName: string;
  period: "all" | "month" | "week" | string;
  maxScore: number;
  totalParticipants: number;
  top: LeaderboardEntry[];
  myEntry?: LeaderboardEntry | null;
};

export type GlobalLeaderboardResponse = {
  period: "all" | "month" | "week" | string;
  maxScoreTotal: number;
  totalParticipants: number;
  top: LeaderboardEntry[];
  myEntry?: LeaderboardEntry | null;
};
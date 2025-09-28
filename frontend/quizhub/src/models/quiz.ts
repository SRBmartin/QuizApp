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

export interface Quiz {
  id: string;
  name: string;
  description?: string | null;
  difficultyLevel: number;   // enum QuizLevel
  timeInSeconds: number;
  createdAt: string;
  isPublished: boolean;
  isDeleted: boolean;
  questions: Question[];
  tags: Tag[];
}

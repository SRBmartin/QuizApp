export enum QuestionType {
  Single = 0,
  Multi = 1,
  TrueFalse = 2,
  FillIn = 3
}

export type QuestionChoice = {
  id?: string;
  label: string;
  isCorrect: boolean;
};

export type Question = {
  id: string;
  quizId: string;
  points: number;
  type: QuestionType;
  question: string;
  createdAt: string;
  isDeleted: boolean;
  choices: QuestionChoice[];
  textAnswer?: { id: string; text: string } | null;
};

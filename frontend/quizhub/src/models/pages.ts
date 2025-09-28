export interface Paged<T> {
  items: T[];
  total: number;
  skip: number;
  take: number;
}
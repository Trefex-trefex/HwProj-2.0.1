export interface IUser {
  firstName: string;
  lastName: string;
  middleName?: string;
  isCritic?: boolean;
  role: Roles;
  userId: number;
}

export enum Roles {
  Student = "student",
  Lecturer = "lecturer",
  Curator = "curator",
}

export interface IFormField<T = string> {
  value: T;
  error: boolean;
  helperText: string;
}

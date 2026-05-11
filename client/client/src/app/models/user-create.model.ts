// create-user.dto.interface.ts
export interface CreateUser {
  name: string;
  password: string;
  email: string;
  phone?: string;
}
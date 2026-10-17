export interface User {
  id: string;
  userName: string;
  email: string;
  displayName: string;
  createdAt: string;
}

export interface AuthSession {
  token: string;
  user: User;
}

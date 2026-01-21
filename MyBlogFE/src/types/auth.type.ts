import type { ApiResponse } from "./common.type";

export interface AuthRequest {
  username: string;
  password: string;
}

export interface RefreshRequest {
  refreshToken: string;
}

export interface RegisterRequest {
  username: string;
  displayName: string;
  dateOfBirth: string; // DateOnly -> ISO date string
  email: string;
  password: string;
}

export interface RegisterData {
  id: string;
  username: string;
  displayName: string;
  dateOfBirth: string;
  email: string;
}

export interface ForgotPasswordRequest {
  identifier: string;
}

export interface ResetPasswordRequest {
  confirmCode: string;
  newPassword: string;
}

export interface RegisterResponse extends ApiResponse<RegisterData> {}

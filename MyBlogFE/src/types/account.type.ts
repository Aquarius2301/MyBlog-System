import type { ApiResponse, PaginationResponse } from "./common.type";

export interface AccountData {
  id: string;
  isOwner: boolean;
  username: string;
  email: string;
  language: string;
  displayName: string;
  dateOfBirth: string;
  avatarUrl: string;
  status: string;
  createdAt: string;
}

export interface AccountNameData {
  id: string;
  avatar: string;
  username: string;
  displayName: string;
  isFollowing: boolean;
  createdAt: string;
}

export interface UpdateAccountRequest {
  // username: string;
  displayName: string;
  dateOfBirth: string;
  language: string;
}

export interface UpdatePasswordRequest {
  oldPassword: string;
  newPassword: string;
}

export interface AccountResponse extends ApiResponse<AccountData> {}
export interface AccountListResponse extends ApiResponse<AccountData[]> {}
export interface AccountNameResponse extends ApiResponse<
  PaginationResponse<AccountNameData>
> {}

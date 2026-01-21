import type {
  AuthRequest,
  ForgotPasswordRequest,
  RegisterRequest,
  RegisterResponse,
  ResetPasswordRequest,
} from "@/types/auth.type";
import apiConfig from "./config.api";
import type { ApiResponse } from "@/types/common.type";

const url = "api/auth";

const authApi = {
  login: async (request: AuthRequest): Promise<ApiResponse<boolean>> => {
    return await apiConfig.post(`${url}/login`, request);
  },

  register: async (request: RegisterRequest): Promise<RegisterResponse> => {
    return await apiConfig.post(`${url}/register`, request);
  },

  confirm: async (
    type: string,
    token: string
  ): Promise<ApiResponse<string>> => {
    return await apiConfig.get(`${url}/confirm?type=${type}&token=${token}`);
  },

  refresh: async (): Promise<ApiResponse<boolean>> => {
    return await apiConfig.post(`${url}/refresh`);
  },

  forgotPassword: async (
    request: ForgotPasswordRequest
  ): Promise<ApiResponse<boolean>> => {
    return await apiConfig.post(`${url}/forgot-password`, request);
  },

  resetPassword: async (
    request: ResetPasswordRequest
  ): Promise<ApiResponse<any>> => {
    return await apiConfig.post(`${url}/reset-password`, request);
  },

  logout: async (): Promise<ApiResponse<boolean>> => {
    return await apiConfig.post(`${url}/logout`);
  },
};

export default authApi;

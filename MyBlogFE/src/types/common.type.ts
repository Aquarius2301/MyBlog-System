import type {
  STATUS_CODE_CLIENT_ERROR,
  STATUS_CODE_ERROR_REQUEST,
  STATUS_CODE_LOST_CONNECTION,
  STATUS_CODE_SERVER_ERROR,
  STATUS_CODE_SUCCESS,
} from "@/constants";

type StatusCodeSuccess = (typeof STATUS_CODE_SUCCESS)[number];
type StatusCodeClientError = (typeof STATUS_CODE_CLIENT_ERROR)[number];
type StatusCodeServerError = (typeof STATUS_CODE_SERVER_ERROR)[number];
type StatusCodeLostConnection = typeof STATUS_CODE_LOST_CONNECTION;
type StatusCodeErrorRequest = typeof STATUS_CODE_ERROR_REQUEST;

export interface ApiResponse<T = any> {
  statusCode:
    | StatusCodeSuccess
    | StatusCodeClientError
    | StatusCodeServerError
    | StatusCodeLostConnection
    | StatusCodeErrorRequest;
  message: string;
  data: T | null;
}

export interface PaginationResponse<T = any> {
  items: T[];
  cursor: string | null;
  pageSize: number;
}

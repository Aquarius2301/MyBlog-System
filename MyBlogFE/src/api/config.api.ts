import type { ApiResponse } from "@/types/common.type";
import axios, { type AxiosRequestConfig } from "axios";
// import { i18n, showErrorLog, showLog } from "@/utils";

const getBaseAPI = import.meta.env.VITE_API_BASE_URL || "http://localhost:5250";

// // Type-safe queue
// interface QueueItem {
//   resolve: (value?: unknown) => void;
//   reject: (reason?: any) => void;
// }

// // variable to handle refresh token
// let isRefreshing = false;
// let failedQueue: QueueItem[] = [];

// // don't use hooks in non-react function, so we create simple log functions
const log = (message: string) => {
  console.log(`[API] ${message}`);
};

const errorLog = (message: string, error?: any) => {
  console.log(`[API Error] ${message}`, error || "");
};

// Function to process the queue of failed requests
// const processQueue = (error: any) => {
//   failedQueue.forEach((prom) => {
//     if (error) {
//       prom.reject(error);
//     } else {
//       prom.resolve();
//     }
//   });
//   failedQueue = [];
// };

const apiConfig = axios.create({
  baseURL: getBaseAPI,
  headers: {
    "Content-Type": "application/json; charset=utf-8",
    // "Accept-Language": i18n.language,
    // Do not set Authorization here to avoid storing old value at file initialization
  },
  timeout: 10000,
  withCredentials: true, // IMPORTANT: To send HttpOnly Cookie
});

// Request Interceptor
apiConfig.interceptors.request.use(
  (config) => {
    // Set dynamic language for each request
    // config.headers["Accept-Language"] = i18n.language;
    log(`Send: ${config.method?.toUpperCase()} ${config.url}`);
    return config;
  },
  (error) => {
    errorLog("Request error:", error);
    return Promise.reject(error);
  }
);

// Response Interceptor
apiConfig.interceptors.response.use(
  (response) => {
    log(
      `Received: ${response.config.method?.toUpperCase()} ${
        response.config.url
      } ${response.status}`
    );

    return response.data;
  },
  async (error) => {
    const originalRequest = error.config as AxiosRequestConfig;

    const errorResponse: ApiResponse<null> = {
      statusCode: error.response.status,
      message: error.response.data?.message,
      data: error.response.data?.data,
    };

    error.response.data = errorResponse;

    if (
      error.response?.status === 401 &&
      !originalRequest.url?.includes("/auth/login")
      // &&
      // !originalRequest.url?.includes("/accounts/profile/me")
    ) {
      errorLog("Response error:", error);
      try {
        const res = await axios.get(`${getBaseAPI}/auth/refresh`);
        if (res.status === 200) {
          log("Token refreshed successfully. Retrying original request.");
          return apiConfig(originalRequest);
        }
      } catch (refreshError) {
        log("Token refresh failed. Redirecting to login.");
        window.location.href = "/login";
      }
    }

    return Promise.reject(error);
  }
);

export default apiConfig;

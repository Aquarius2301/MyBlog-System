import apiConfig from "./config.api";
import type { ApiResponse } from "@/types/common.type";

const url = "api/upload";

const uploadApi = {
  upload: async (request: FormData) => {
    let path = `${url}`;
    return (await apiConfig.post(path, request, {
      headers: { "Content-Type": "multipart/form-data" },
      timeout: 120000,
    })) as ApiResponse<string[]>;
  },
};

export default uploadApi;

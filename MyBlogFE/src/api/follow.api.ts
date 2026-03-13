import type { ApiResponse } from "@/types/common.type";
import apiConfig from "./config.api";

const url = "api/follows";

const accountApi = {
  follow: async (accountId: string): Promise<ApiResponse<null>> => {
    return await apiConfig.post(`${url}/${accountId}`);
  },

  unfollow: async (accountId: string): Promise<ApiResponse<null>> => {
    return await apiConfig.delete(`${url}/${accountId}`);
  },
};

export default accountApi;

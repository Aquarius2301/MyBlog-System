import type { ApiResponse } from "@/types/common.type";
import apiConfig from "./config.api";
import type { AccountNameResponse } from "@/types/account.type";

const baseUrl = "api/follows";

const pageSize = 5;

const accountApi = {
  follow: async (accountId: string): Promise<ApiResponse<number>> => {
    return await apiConfig.post(`${baseUrl}/${accountId}`);
  },

  unfollow: async (accountId: string): Promise<ApiResponse<number>> => {
    return await apiConfig.delete(`${baseUrl}/${accountId}`);
  },

  getFollowers: async (
    id: string,
    cursor?: number,
  ): Promise<AccountNameResponse> => {
    let url = `${baseUrl}/${id}/followers?pageSize=${pageSize}`;
    if (cursor) {
      url += `&cursor=${cursor}`;
    }
    return await apiConfig.get(url);
  },
};

export default accountApi;

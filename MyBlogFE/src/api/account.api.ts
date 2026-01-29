import type {
  AccountNameResponse,
  AccountResponse,
} from "@/types/account.type";
import apiConfig from "./config.api";
import type { ApiResponse } from "@/types/common.type";

const url = "api/accounts";

const pageSize = Number(import.meta.env.VITE_PAGE_SIZE) || 10;

const accountApi = {
  getMyProfile: async (): Promise<AccountResponse> => {
    return await apiConfig.get(`${url}/profile/me`);
  },

  getProfileByUsername: async (username: string): Promise<AccountResponse> => {
    return await apiConfig.get(`${url}/profile/username/${username}`);
  },

  searchAccounts: async (
    name: string,
    cursor: string | null,
  ): Promise<AccountNameResponse> => {
    let path = `${url}/search?name=${encodeURIComponent(name)}&pageSize=${pageSize}`;
    if (cursor) {
      path = path.concat(`&cursor=${cursor}`);
    }
    return await apiConfig.get(path);
  },

  changeAvatar: async (picture: string): Promise<ApiResponse<any>> => {
    return await apiConfig.put(`${url}/profile/me/change-avatar`, {
      picture,
    });
  },
};

export default accountApi;

import type {
  AccountNameResponse,
  AccountResponse,
} from "@/types/account.type";
import apiConfig from "./config.api";
import type { ApiResponse } from "@/types/common.type";

const url = "api/accounts";

const accountApi = {
  getMyProfile: async (): Promise<AccountResponse> => {
    return await apiConfig.get(`${url}/profile/me`);
  },

  getProfileByUsername: async (username: string): Promise<AccountResponse> => {
    return await apiConfig.get(`${url}/profile/username/${username}`);
  },

  searchAccounts: async (
    name: string,
    cursor: string | null
  ): Promise<AccountNameResponse> => {
    return await apiConfig.get(
      `${url}?name=${encodeURIComponent(name)}${
        cursor ? `&cursor=${encodeURIComponent(cursor)}` : ""
      }`
    );
  },

  changeAvatar: async (picture: string): Promise<ApiResponse<any>> => {
    return await apiConfig.put(`${url}/profile/me/change-avatar`, {
      picture,
    });
  },
};

export default accountApi;

import type {
  CustomTarotRequest,
  GuidedTarotRequest,
  TarotReadingResponse,
} from "@/types/tarot.type";
import apiConfig from "./config.api";
import type { ApiResponse } from "@/types/common.type";

const url = "api/tarot";

const tarotApi = {
  getTarot: async (): Promise<ApiResponse<TarotReadingResponse[]>> => {
    return await apiConfig.get(url);
  },
  getGuidedTarotReading: async (
    request: GuidedTarotRequest,
  ): Promise<ApiResponse<any>> => {
    return await apiConfig.post(url + "/guided", request);
  },
  getCustomTarotReading: async (
    request: CustomTarotRequest,
  ): Promise<ApiResponse<any>> => {
    return await apiConfig.post(url + "/custom", request);
  },
};

export default tarotApi;

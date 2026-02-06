import type {
  TarotReadingRequest,
  TarotReadingResponse,
} from "@/types/tarot.type";
import apiConfig from "./config.api";
import type { ApiResponse } from "@/types/common.type";

const url = "api/tarot";

const tarotApi = {
  getTarot: async (): Promise<ApiResponse<TarotReadingResponse[]>> => {
    return await apiConfig.get(url);
  },
  getTarotReading: async (
    request: TarotReadingRequest,
  ): Promise<ApiResponse<any>> => {
    return await apiConfig.post(url, request);
  },
};

export default tarotApi;

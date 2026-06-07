import type { ConversationsResponse, MessageResponse } from "@/types/chat.type";
import apiConfig from "./config.api";

const baseURL = "/api/conversations";
const pageSize = 20;

const chatApi = {
  getAllConversations: async (): Promise<ConversationsResponse> => {
    return await apiConfig.get(`${baseURL}?pageSize=${pageSize}`);
  },

  getMessages: async (
    conversationId: string,
    cursor: string | null,
  ): Promise<MessageResponse> => {
    if (cursor) {
      return await apiConfig.get(
        `${baseURL}/${conversationId}/messages?cursor=${cursor}`,
      );
    }
    return await apiConfig.get(`${baseURL}/${conversationId}/messages`);
  },
};

export default chatApi;

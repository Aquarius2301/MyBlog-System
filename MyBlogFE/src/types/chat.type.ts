import type { AccountNameData } from "./account.type";
import type { ApiResponse, PaginationResponse } from "./common.type";

export type LastMessageData = {
  messageId: string;
  isOwner: boolean;
  content: string;
  isRead: boolean;
  createdAt: string;
};

export type ConversationsData = {
  conversationId: string;
  account: AccountNameData;
  lastMessage: LastMessageData | null;
};

export type MessagesData = {
  id: string;
  senderId: string;
  content: string;
  createdAt: string;
};

export type MessageResponse = ApiResponse<PaginationResponse<MessagesData>>;

export type ConversationsResponse = ApiResponse<ConversationsData[]>;

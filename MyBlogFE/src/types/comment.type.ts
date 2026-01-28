import type { AccountNameData } from "./account.type";
import type { ApiResponse, PaginationResponse } from "./common.type";

export type GetCommentsData = {
  id: string;
  postId: string;
  parentCommentId: string | null;
  commenter: AccountNameData;
  replyAccount: AccountNameData | null;
  content: string;
  pictures: string[];
  likeCount: number;
  commentCount: number;
  isLiked: boolean;
  createdAt: string;
  updatedAt: string | null;
  isOwner: boolean;
};

export type CreateCommentResponse = ApiResponse<GetCommentsData>;

export type GetCommentsResponse = ApiResponse<
  PaginationResponse<GetCommentsData>
>;

export type CreateCommentRequest = {
  postId?: string;
  parentCommentId?: string;
  content: string;
  pictures: string[];
  replyAccountId?: string;
};

export type CreateCommentData = {
  id: string;
  accountId: string;
  content: string;
  parentCommentId: string | null;
  postId: string;
  replyAccountId: string | null;
  pictures: string[];
  createdAt: string;
};

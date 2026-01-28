import type {
  CreateCommentRequest,
  CreateCommentResponse,
  GetCommentsResponse,
} from "@/types/comment.type";
import apiConfig from "./config.api";
import type { ApiResponse } from "@/types/common.type";

const url = "api/comments";
const pageSize = Number(import.meta.env.VITE_PAGE_SIZE) || 5;

const commentApi = {
  getChildComments: async (
    commentId: string,
    cursor: string | null,
  ): Promise<GetCommentsResponse> => {
    let path = `${url}/${commentId}?pageSize=${pageSize}`;
    if (cursor) {
      path = path.concat(`&cursor=${cursor}`);
    }
    return await apiConfig.get(path);
  },

  likeComment: async (commentId: string): Promise<ApiResponse<number>> => {
    let path = `${url}/${commentId}/like`;
    return await apiConfig.post(path);
  },

  cancelLikeComment: async (
    commentId: string,
  ): Promise<ApiResponse<number>> => {
    let path = `${url}/${commentId}/cancel-like`;
    return await apiConfig.delete(path);
  },

  addComment: async (
    request: CreateCommentRequest,
  ): Promise<CreateCommentResponse> => {
    let path = `${url}`;
    return await apiConfig.post(path, request);
  },

  deleteComment: async (commentId: string): Promise<any> => {
    let path = `${url}/${commentId}`;
    return await apiConfig.delete(path);
  },
};

export default commentApi;

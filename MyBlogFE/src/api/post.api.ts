import type { ApiResponse } from "@/types/common.type";
import type {
  CreatePostRequest,
  GetPostDetailResponse,
  GetPostsResponse,
  UpdatePostRequest,
} from "@/types/post.type";
import type { GetCommentsResponse } from "@/types/comment.type";
import apiConfig from "./config.api";

const url = "api/posts";

const pageSize = Number(import.meta.env.VITE_PAGE_SIZE) || 5;

const postApi = {
  getPosts: async (cursor: string | null): Promise<GetPostsResponse> => {
    let path = `${url}?pageSize=${pageSize}`;
    if (cursor) {
      path = path.concat(`&cursor=${cursor}`);
    }

    return await apiConfig.get(path);
  },

  getMyPosts: async (cursor: string | null) => {
    let path = `${url}/me?pageSize=${pageSize}`;
    if (cursor) {
      path = path.concat(`&cursor=${cursor}`);
    }

    return (await apiConfig.get(path)) as GetPostsResponse;
  },

  getPostByUsername: async (
    username: string,
    cursor: string | null
  ): Promise<GetPostsResponse> => {
    let path = `${url}/username/${username}?pageSize=${pageSize}`;
    if (cursor) {
      path = path.concat(`&cursor=${cursor}`);
    }
    return await apiConfig.get(path);
  },

  getPostByLink: async (link: string): Promise<GetPostDetailResponse> => {
    let path = `${url}/link/${link}`;
    return await apiConfig.get(path);
  },

  getPostComments: async (
    postId: string,
    cursor: string | null
  ): Promise<GetCommentsResponse> => {
    let path = `${url}/${postId}/comments?pageSize=${pageSize}`;
    if (cursor) {
      path = path.concat(`&cursor=${cursor}`);
    }
    return await apiConfig.get(path);
  },

  likePost: async (postId: string): Promise<ApiResponse<number>> => {
    let path = `${url}/${postId}/like`;
    return await apiConfig.post(path);
  },

  cancelLikePost: async (postId: string): Promise<ApiResponse<number>> => {
    let path = `${url}/${postId}/cancel-like`;
    return await apiConfig.delete(path);
  },

  createPost: async (
    request: CreatePostRequest
  ): Promise<GetPostDetailResponse> => {
    let path = `${url}`;
    return await apiConfig.post(path, request, {});
  },

  updatePost: async (
    request: UpdatePostRequest,
    id: string
  ): Promise<GetPostDetailResponse> => {
    let path = `${url}/${id}`;
    return await apiConfig.put(path, request);
  },

  deletePost: async (id: string): Promise<ApiResponse<null>> => {
    let path = `${url}/${id}`;
    return await apiConfig.delete(path);
  },
};

export default postApi;

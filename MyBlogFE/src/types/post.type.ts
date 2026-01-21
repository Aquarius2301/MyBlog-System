import type { ApiResponse, PaginationResponse } from "./common.type";
import type { AccountData, AccountNameData } from "./account.type";

interface PostLatestComment {
  commenter: AccountData;
  content: string;
  createdAt: string;
  updatedAt: string | null;
}

export interface GetPostsData extends GetPostDetailData {
  latestComment: PostLatestComment | null;
  score: number;
}

export interface GetPostsResponse
  extends ApiResponse<PaginationResponse<GetPostsData>> {}

export interface GetPostDetailData {
  id: string;
  link: string;
  content: string;
  author: AccountNameData;
  postPictures: string[];
  isLiked: boolean;
  likeCount: number;
  commentCount: number;
  isOwner: boolean;
  createdAt: string;
  updatedAt: string | null;
}

export interface GetPostDetailResponse extends ApiResponse<GetPostDetailData> {}

export interface CreatePostRequest {
  content: string;
  pictures: string[];
}

export interface CreatePostData {
  id: string;
  link: string;
  content: string;
  pictures: string[];
  createdAt: string;
}

export interface CreatePostResponse extends ApiResponse<CreatePostData> {}

export interface UpdatePostRequest {
  content: string;
  pictures: string[];
}

export interface UpdatePostData {
  id: string;
  link: string;
  content: string;
  postPictures: string[];
  updatedAt: string;
}

export interface UpdatePostResponse extends ApiResponse<UpdatePostData> {}

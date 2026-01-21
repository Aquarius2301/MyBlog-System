import { postApi } from "@/api";
import { ImageAvatar, ImageList, Paragraph, Text } from "@/components";
import {
  useApiMutation,
  useFixInfiniteQuery,
  useSafeTranslation,
} from "@/hooks";
import { formatDateTime } from "@/utils";
import { Button, Card, Divider, Flex, Space } from "antd";
import { useNavigate } from "react-router-dom";
import { CommentOutlined, LikeFilled, LikeOutlined } from "@ant-design/icons";
import { useState } from "react";
import { DeletePostModal, UpdatePostModal, ViewPostModal } from "./modals";
import { PostEditDropdown } from "./PostEditDropdown";
import type { GetPostDetailResponse, GetPostsData } from "@/types/post.type";
import { useQueryClient } from "@tanstack/react-query";

export type PostItemProps = {
  post: GetPostsData;
  enableViewModal?: boolean;
};

type ModalState = "view" | "update" | "delete" | null;

const PostItem = ({ post, enableViewModal = true }: PostItemProps) => {
  const navigate = useNavigate();
  const { t } = useSafeTranslation();
  const [activeModal, setActiveModal] = useState<ModalState>(null);
  const queryClient = useQueryClient();

  // const { likePost, unlikePost } = usePost();
  const { updateItem } = useFixInfiniteQuery<GetPostsData>({
    keySelector: (item) => item.id,
  });

  const { mutate: likePostApi, isLoading: likePostLoading } =
    useApiMutation<number>({
      mutationKey: ["likePost", post.id],
      mutationFn: postApi.likePost,
      onSuccess: (data) => {
        queryClient.setQueryData(
          ["getPostByLink", post.link],
          (oldData: GetPostDetailResponse) => {
            if (!oldData) return oldData;
            return {
              ...oldData,
              data: {
                ...oldData.data,
                isLiked: true,
                likeCount: data,
              },
            };
          }
        );
        updateItem(["posts"], post.id, (oldPost) => ({
          ...oldPost,
          isLiked: true,
          likeCount: data!,
        }));
      },
    });

  const { mutate: cancelLikePostApi, isLoading: cancelLikePostLoading } =
    useApiMutation<number>({
      mutationKey: ["cancelLikePost", post.id],
      mutationFn: postApi.cancelLikePost,
      onSuccess: (data) => {
        queryClient.setQueryData(
          ["getPostByLink", post.link],
          (oldData: GetPostDetailResponse) => {
            if (!oldData) return oldData;
            return {
              ...oldData,
              data: {
                ...oldData.data,
                isLiked: false,
                likeCount: data,
              },
            };
          }
        );
        updateItem(["posts"], post.id, (oldPost) => ({
          ...oldPost,
          isLiked: false,
          likeCount: data!,
        }));
      },
    });

  const handleLike = () => {
    if (cancelLikePostLoading || likePostLoading) return;
    post.isLiked ? cancelLikePostApi(post.id) : likePostApi(post.id);
  };

  // console.log("PostItem rendered", post);

  return (
    post && (
      <Card style={{ width: "100%", marginBottom: "20px" }}>
        {/* Avatar, account name and post date */}
        <Flex justify="space-between" align="center">
          <Flex
            justify="flex-start"
            align="center"
            style={{ marginBottom: "20px" }}
          >
            <div>
              <ImageAvatar
                url={post.author.avatar}
                shape="square"
                size="large"
              />
            </div>
            <div
              style={{ marginLeft: "10px", cursor: "pointer" }}
              onClick={() =>
                navigate(`/profile?username=${post.author.username}`)
              }
            >
              <Text bold as={"p"}>
                {post.author.displayName}{" "}
                {post.author.isFollowing ? `(${t("Following")})` : ""}
              </Text>
              <Text fontSize="small" color="gray" as={"p"}>
                {t("PostDate")}: {formatDateTime(post.createdAt)}
              </Text>
            </div>
          </Flex>
          {post.isOwner && (
            <PostEditDropdown
              post={post}
              onUpdate={() => setActiveModal("update")}
              onDelete={() => setActiveModal("delete")}
            />
          )}
        </Flex>
        {/* Content */}
        <Paragraph content={post.content} isExpandable />
        {/* Images */}
        <ImageList pictureUrls={post.postPictures} />
        {/* Likes and Comments */}
        <Divider />
        <Space size="small">
          <Button
            onClick={handleLike}
            loading={likePostLoading || cancelLikePostLoading}
            disabled={likePostLoading || cancelLikePostLoading}
            icon={post.isLiked ? <LikeFilled /> : <LikeOutlined />}
          >
            {post.likeCount}
          </Button>

          <Button
            type={enableViewModal ? "default" : "text"}
            onClick={() => {
              enableViewModal && setActiveModal("view");
            }}
            icon={<CommentOutlined />}
            style={{ cursor: enableViewModal ? "pointer" : "default" }}
          >
            {post.commentCount}
          </Button>
        </Space>

        {/* Lastest Comment */}
        {post.latestComment && enableViewModal && (
          <Card
            style={{ marginTop: "20px", cursor: "pointer" }}
            onClick={() => setActiveModal("view")}
          >
            <Flex align="center" gap="10px">
              <ImageAvatar url={post.latestComment.commenter.avatarUrl} />
              <Text bold fontSize="small">
                {post.latestComment.commenter.displayName} (@
                {post.latestComment.commenter.username}):{" "}
                <Text fontSize="small">{post.latestComment.content}</Text>
              </Text>
            </Flex>
          </Card>
        )}

        {activeModal === "view" && (
          <ViewPostModal
            link={post.link}
            isModalOpen={true}
            onClose={() => {
              setActiveModal(null);
            }}
          />
        )}

        {activeModal === "update" && (
          <UpdatePostModal
            id={post.id}
            link={post.link}
            isModalOpen={true}
            onClose={() => setActiveModal(null)}
          />
        )}

        <DeletePostModal
          id={post.id}
          isModalOpen={activeModal === "delete"}
          onClose={() => setActiveModal(null)}
        />
      </Card>
    )
  );
};

export default PostItem;

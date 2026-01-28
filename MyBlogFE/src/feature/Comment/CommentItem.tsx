import { commentApi } from "@/api";
import { ImageAvatar, ImageList, Paragraph, Text } from "@/components";
import {
  useSafeTranslation,
  useApiMutation,
  useFixInfiniteQuery,
} from "@/hooks";
import { formatDateTime } from "@/utils";
import { Button, Divider, Flex, Modal, Space } from "antd";
import { CommentOutlined, LikeFilled, LikeOutlined } from "@ant-design/icons";
import CommentChildList from "./CommentChildList";
import CommentCreateInput from "./components/CommentCreateInput";
import { CommentProvider } from "@/contexts/CommentContext";
import type { GetCommentsData } from "@/types/comment.type";
import { CommentEditDropdown } from "./CommentEditDropdown";
import { useState } from "react";
import { DeleteCommentModal } from "./components/DeleteCommentModal";

type CommentItemProps = {
  replyingToId: string | null | undefined;
  setReplyingToId: (id: string | null | undefined) => void;
  postId: string;
  item: GetCommentsData;
  // onReturn: (item: GetCommentsData) => void;
};

type ModalState = "update" | "delete" | null;

const CommentItem = ({
  postId,
  item,
  // onReturn,
  replyingToId,
  setReplyingToId,
}: CommentItemProps) => {
  const [activeModal, setActiveModal] = useState<ModalState>(null);

  const { updateItem } = useFixInfiniteQuery<GetCommentsData>({
    keySelector: (item) => item.id,
  });

  const { mutate: likeCommentApi, isLoading: likeCommentLoading } =
    useApiMutation<number, string>({
      mutationKey: ["likeComment", item.id],
      mutationFn: commentApi.likeComment,
      onSuccess: (data) => {
        updateItem(["getPostComments", postId], item.id, (oldItem) => ({
          ...oldItem,
          isLiked: true,
          likeCount: data!,
        }));
      },
    });

  const { mutate: cancelLikeCommentApi, isLoading: cancelLikeCommentLoading } =
    useApiMutation<number, string>({
      mutationKey: ["cancelLikeComment", item.id],
      mutationFn: commentApi.cancelLikeComment,
      onSuccess: (data) => {
        updateItem(["getPostComments", postId], item.id, (oldItem) => ({
          ...oldItem,
          isLiked: false,
          likeCount: data!,
        }));
      },
    });

  const handleLike = () => {
    if (cancelLikeCommentLoading || likeCommentLoading) return;

    item.isLiked ? cancelLikeCommentApi(item.id) : likeCommentApi(item.id);
  };

  const { t } = useSafeTranslation();

  return (
    <>
      <Divider />
      <Flex
        justify="space-between"
        align="center"
        style={{ marginBottom: "20px" }}
      >
        <div>
          <ImageAvatar url={item.commenter.avatar} />
          <Text bold style={{ marginLeft: "10px", display: "inline-block" }}>
            {item.commenter.displayName}
          </Text>
          <Text color="gray" fontSize={"small"}>
            {" "}
            @{item.commenter.username}
          </Text>
        </div>
        <div>
          {t("CommentDate")}: {formatDateTime(item.createdAt)}{" "}
          {item.isOwner && (
            <CommentEditDropdown
              onUpdate={() => {
                setActiveModal("update");
              }}
              onDelete={() => {
                setActiveModal("delete");
              }}
            />
          )}
        </div>
      </Flex>
      {/* Content */}
      <Paragraph content={item.content} isExpandable />
      {/* Images */}
      <ImageList pictureUrls={item.pictures} />
      <Space size="large" style={{ margin: "10px 0" }}>
        <div onClick={handleLike} style={{ cursor: "pointer" }}>
          {/* <Text>
            {item.isLiked ? <LikeFilled /> : <LikeOutlined />} {item.likeCount}
          </Text> */}
          <Button
            onClick={handleLike}
            loading={likeCommentLoading || cancelLikeCommentLoading}
            disabled={likeCommentLoading || cancelLikeCommentLoading}
            icon={item.isLiked ? <LikeFilled /> : <LikeOutlined />}
          >
            {item.likeCount}
          </Button>
        </div>
        <div
          onClick={() => setReplyingToId(item.id)}
          style={{ cursor: "pointer" }}
        >
          <Text>{<CommentOutlined />}</Text>
        </div>
      </Space>
      {activeModal === "update" && <Modal></Modal>}
      {activeModal === "delete" && (
        <DeleteCommentModal
          id={item.id}
          postId={postId}
          onClose={() => setActiveModal(null)}
        />
      )}
      {replyingToId == item.id && (
        <CommentCreateInput
          comment={item}
          postId={postId}
          parentCommentId={item.id}
        />
      )}
      {item.commentCount > 0 && (
        <CommentProvider>
          <CommentChildList
            commentId={item.id}
            postId={postId}
            replyingToId={replyingToId}
            setReplyingToId={setReplyingToId}
            parentCommentId={item.id}
            // onCreateReturn()
          />
        </CommentProvider>
      )}
    </>
  );
};

export default CommentItem;

import { commentApi } from "@/api";
import { ImageAvatar, ImageList, Paragraph, Text } from "@/components";
import {
  useSafeTranslation,
  useApiMutation,
  useFixInfiniteQuery,
} from "@/hooks";
import { formatDateTime } from "@/utils";
import { Button, Divider, Flex, Space } from "antd";
import {
  CommentOutlined,
  LikeFilled,
  LikeOutlined,
  SwapRightOutlined,
} from "@ant-design/icons";
import CommentCreateInput from "./components/CommentCreateInput";
import type { GetCommentsData } from "@/types/comment.type";

type CommentItemProps = {
  item: GetCommentsData;
  parentCommentId: string;
  // onUpdateReturn: (item: GetCommentsData) => void;
  // onCreateReturn: (item: CreateCommentData) => void;
  replyingToId: string | null | undefined;
  setReplyingToId: (id: string | null | undefined) => void;
  postId: string;
};

const CommentChildItem = ({
  item,
  // onUpdateReturn,
  // onCreateReturn,
  replyingToId,
  setReplyingToId,
  postId,
  parentCommentId,
}: CommentItemProps) => {
  // const { likeComment, unlikeComment, addUnderComment } = useComment();

  const { updateItem } = useFixInfiniteQuery<GetCommentsData>({
    keySelector: (item) => item.id,
  });

  const { mutate: likeCommentApi, isLoading: likeCommentLoading } =
    useApiMutation<number, string>({
      mutationKey: ["likeComment", item.id],
      mutationFn: commentApi.likeComment,
      onSuccess: (data) => {
        updateItem(
          ["getChildComments", parentCommentId],
          item.id,
          (oldItem) => ({
            ...oldItem,
            isLiked: true,
            likeCount: data!,
          })
        );
      },
    });

  const { mutate: cancelLikeCommentApi, isLoading: cancelLikeCommentLoading } =
    useApiMutation<number, string>({
      mutationKey: ["cancelLikeComment", item.id],
      mutationFn: commentApi.cancelLikeComment,
      onSuccess: (data) => {
        updateItem(
          ["getChildComments", parentCommentId],
          item.id,
          (oldItem) => ({
            ...oldItem,
            isLiked: false,
            likeCount: data!,
          })
        );
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
          </Text>{" "}
          <SwapRightOutlined />
          <Text bold style={{ marginLeft: "10px", display: "inline-block" }}>
            {item.replyAccount?.displayName}
          </Text>
          <Text color="gray" fontSize={"small"}>
            {" "}
            @{item.replyAccount?.username}
          </Text>
        </div>
        {t("CommentDate")}: {formatDateTime(item.createdAt)}
      </Flex>
      {/* Content */}
      <Paragraph content={item.content} isExpandable />
      {/* Images */}
      <ImageList pictureUrls={item.pictures} />
      <Space size="large" style={{ margin: "10px 0" }}>
        <Button
          onClick={handleLike}
          loading={likeCommentLoading || cancelLikeCommentLoading}
          disabled={likeCommentLoading || cancelLikeCommentLoading}
          icon={item.isLiked ? <LikeFilled /> : <LikeOutlined />}
        >
          {item.likeCount}
        </Button>
        <div
          onClick={() => setReplyingToId(item.id)}
          style={{ cursor: "pointer" }}
        >
          <Text>{<CommentOutlined />}</Text>
        </div>
      </Space>

      {replyingToId == item.id && (
        <CommentCreateInput
          parentCommentId={parentCommentId}
          comment={item}
          postId={postId}
        />
      )}
    </>
  );
};

export default CommentChildItem;

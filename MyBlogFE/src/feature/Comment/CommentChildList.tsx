import { useSafeTranslation, useApiInfiniteQuery } from "@/hooks";
import { Button, Space } from "antd";
import CommentChildItem from "./CommentChildItem";
import type { GetCommentsData } from "@/types/comment.type";
import { commentApi } from "@/api";

type CommentChildListProps = {
  commentId: string;
  postId: string;
  replyingToId: string | null | undefined;
  setReplyingToId: (id: string | null | undefined) => void;
  parentCommentId: string;
};

const CommentChildList = ({
  commentId,
  postId,
  replyingToId,
  setReplyingToId,
  parentCommentId,
}: CommentChildListProps) => {
  const { t } = useSafeTranslation();

  const { isLoading, data, fetchNextPage, hasNextPage } =
    useApiInfiniteQuery<GetCommentsData>({
      queryKey: ["getChildComments", commentId],
      queryFn: (pageParam) =>
        commentApi.getChildComments(commentId, pageParam ?? null),
      initialPageParam: null,
      enabled: false,
    });

  return (
    <div
      style={{
        marginTop: "10px",
        marginLeft: "16px",
        borderLeft: "2px solid #4c4b4bff",
        paddingLeft: "16px",
      }}
    >
      {data && data.length > 0 && (
        <Space
          vertical
          size={8}
          style={{ width: "100%" }}
          styles={{ item: { marginLeft: "16px", marginRight: "16px" } }}
        >
          {data.map((item) => (
            <CommentChildItem
              item={item}
              postId={postId}
              replyingToId={replyingToId}
              setReplyingToId={setReplyingToId}
              parentCommentId={parentCommentId}
              key={item.id}
            />
          ))}
        </Space>
      )}

      {(!data || hasNextPage) && (
        <Button
          type="text"
          loading={isLoading}
          disabled={isLoading}
          onClick={() => fetchNextPage()}
        >
          {t("ShowReply")}
        </Button>
      )}

      {data && hasNextPage === false && <b>{t("NoMoreReplies")} </b>}
    </div>
  );
};

export default CommentChildList;

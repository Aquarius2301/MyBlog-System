import { postApi } from "@/api";
import { useSafeTranslation, useApiInfiniteQuery } from "@/hooks";
import { useState } from "react";
import CommentItem from "./CommentItem";
import { Button, Empty, Space, Spin } from "antd";
import CommentCreateInput from "./components/CommentCreateInput";
import InfiniteScroll from "react-infinite-scroll-component";
import type { GetCommentsData } from "@/types/comment.type";

type CommentListProps = {
  postId: string;
  // onCreateReturn: (item: GetCommentsData) => void;
};

const CommentList = ({ postId }: CommentListProps) => {
  // const [cursor, setCursor] = useState<string | null>(null);
  // const [hasMore, setHasMore] = useState<boolean>(true);
  const [replyingToId, setReplyingToId] = useState<string | null | undefined>(
    undefined
  );

  // const { comments, setComment, onResetComments, addComment } = useComment();

  const { t } = useSafeTranslation();

  // const { loading, execute } = useApi<PaginationResponse<GetCommentsData>>(
  //   postApi.getPostComments,
  //   {
  //     onSuccess: ({ data }) => {
  //       handleData(data);
  //     },
  //   }
  // );

  // const { isLoading: loading, refetch } = useApiQuery<
  //   PaginationResponse<GetCommentsData>
  // >({
  //   queryKey: ["getPostComments", postId, cursor],
  //   queryFn: () => postApi.getPostComments(postId, cursor),
  //   onSuccess: (data) => {
  //     data && handleData(data);
  //   },
  // });

  const { isLoading, data, fetchNextPage, hasNextPage } =
    useApiInfiniteQuery<GetCommentsData>({
      queryKey: ["getPostComments", postId],
      queryFn: (pageParam) =>
        postApi.getPostComments(postId, pageParam ?? null),
      initialPageParam: null,
    });

  // const handleData = (data: PaginationResponse<GetCommentsData>) => {
  //   setComment(data.items);
  //   setCursor(data.cursor);
  //   setHasMore(data.items.length === 5);
  // };

  // const fetchComments = async () => {
  //   if (loading || !hasMore) return;
  //   refetch();
  // };

  // useEffect(() => {
  //   onResetComments();
  //   setCursor(null);
  //   setHasMore(true);
  //   fetchComments();
  // }, []);

  return isLoading ? (
    <Spin />
  ) : (
    <>
      <Space
        vertical
        size={8}
        style={{ width: "100%" }}
        styles={{ item: { marginLeft: "16px", marginRight: "16px" } }}
      >
        {replyingToId === null ? (
          <CommentCreateInput
            comment={null}
            postId={postId}
            // onCreateReturn={(item) => {
            //   return addComment(item);
            //   // onCreateReturn(newComment);
            // }}
          />
        ) : (
          <Button type="primary" onClick={() => setReplyingToId(null)}>
            Create Comment
          </Button>
        )}
        {data && data.length === 0 ? (
          <Empty description={t("NoComments")} />
        ) : (
          <InfiniteScroll
            dataLength={data?.length || 0}
            next={fetchNextPage}
            hasMore={hasNextPage}
            loader={<Spin />}
            endMessage={
              <p style={{ textAlign: "center" }}>
                <b>{t("NoMoreComments")}</b>
              </p>
            }
          >
            {data?.map((item) => (
              <CommentItem
                key={item.id}
                replyingToId={replyingToId}
                setReplyingToId={setReplyingToId}
                item={item}
                postId={postId}
              />
            ))}
          </InfiniteScroll>
        )}
      </Space>
    </>
  );
};

export default CommentList;

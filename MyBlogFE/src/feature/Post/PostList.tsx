import { useApiInfiniteQuery, useSafeTranslation } from "@/hooks";
import type { GetPostsData, GetPostsResponse } from "@/types/post.type";
import { useState } from "react";
import PostLoadingCard from "./PostLoadingCard";
import { Button } from "antd";
import { CreatePostModal } from "./modals";
import InfiniteScroll from "react-infinite-scroll-component";
import PostItem from "./PostItem";

export type PostListProps = {
  fetch: (...args: any[]) => Promise<GetPostsResponse>;
  showCreatePost?: boolean;
};

const PostList = ({ fetch, showCreatePost = true }: PostListProps) => {
  const { t } = useSafeTranslation();
  // const [initialLoading, setInitialLoading] = useState<boolean>(true);
  const [createPostModal, setCreatePostModal] = useState<boolean>(false);

  const {
    data: posts,
    fetchNextPage,
    hasNextPage,
  } = useApiInfiniteQuery<GetPostsData, string | null>({
    queryKey: ["posts"],
    queryFn: (pageParam) => fetch(pageParam),
    initialPageParam: null,
  });

  return (
    <>
      {showCreatePost && (
        <Button
          style={{ width: "100%", marginBottom: 20 }}
          onClick={() => {
            setCreatePostModal(true);
          }}
        >
          {t("CreatePost")}
        </Button>
      )}
      <CreatePostModal
        modalVisible={createPostModal}
        onClose={() => {
          setCreatePostModal(false);
        }}
      />
      <InfiniteScroll
        dataLength={posts?.length || 0}
        next={fetchNextPage}
        hasMore={hasNextPage || posts === null}
        loader={<PostLoadingCard isList />}
        scrollThreshold={0.95}
        endMessage={
          <p style={{ textAlign: "center" }}>
            <b>{t("NoMorePostsToLoad")}</b>
          </p>
        }
      >
        {posts?.map((post) => post && <PostItem key={post.id} post={post} />)}
      </InfiniteScroll>
    </>
  );
};

export default PostList;

import { App, Modal } from "antd";
import { useApiQuery, useSafeTranslation, useFixInfiniteQuery } from "@/hooks";
import PostLoadingCard from "../PostLoadingCard";
import PostItem from "../PostItem";
import { CommentList } from "@/feature/Comment";
import { CommentProvider } from "@/contexts/CommentContext";
import type {
  GetPostDetailData,
  GetPostsData,
  GetPostsResponse,
} from "@/types/post.type";
import { postApi } from "@/api";
import { useQueryClient, type InfiniteData } from "@tanstack/react-query";
import { useEffect, useMemo } from "react";

type ViewPostModalProps = {
  link: string;
  isModalOpen: boolean;
  onClose: () => void;
  // onCommentCreated: (comment: GetCommentsData) => void;
};

const ViewPostModal = ({
  link,
  isModalOpen,
  onClose,
}: // onCommentCreated,
ViewPostModalProps) => {
  const { message } = App.useApp();
  const { t } = useSafeTranslation();
  const queryClient = useQueryClient();

  const setData = (p: GetPostsData, data: GetPostDetailData): GetPostsData => {
    return {
      ...p,
      author: data.author,
      createdAt: data.createdAt,
      id: data.id,
      link: data.link,
      isOwner: data.isOwner,
      content: data.content,
      postPictures: data.postPictures,
      updatedAt: data.updatedAt,
      isLiked: data.isLiked,
      likeCount: data.likeCount,
      commentCount: data.commentCount,
    };
  };

  const { updateItem } = useFixInfiniteQuery<GetPostsData>({
    keySelector: (item) => item.id,
  });

  const {
    data,
    isLoading: loading,
    // refetch,
  } = useApiQuery<GetPostDetailData>({
    queryKey: ["getPostByLink", link],
    queryFn: () => postApi.getPostByLink(link),
    enabled: isModalOpen && !!link && link.length > 0,
  });

  useEffect(() => {
    if (!data) return;

    // Since onSucces is deprecated, we use useEffect to update the post data in the cache
    updateItem(["posts"], data.id, (old) => setData(old, data));
  }, [data, link, queryClient]);

  useEffect(() => {
    if (!data && isModalOpen && !loading) {
      message.error(t("PostNotFound"));
      onClose();
    }
  }, [data, isModalOpen, message, t, onClose]);

  const latestComment = useMemo(() => {
    return (
      queryClient
        .getQueryData<InfiniteData<GetPostsResponse>>(["posts"])
        ?.pages.flatMap((p) => p.data?.items)
        .find((p) => p?.id === data?.id)?.latestComment ?? null
    );
  }, [data]);

  const score = useMemo(() => {
    return (
      queryClient
        .getQueryData<InfiniteData<GetPostsResponse>>(["posts"])
        ?.pages.flatMap((p) => p.data?.items)
        .find((p) => p?.id === data?.id)?.score ?? 0
    );
  }, [data]);

  return (
    <Modal
      title={t("PostDetails")}
      open={isModalOpen}
      footer={null}
      width={1200}
      onCancel={() => onClose()}
      closeIcon={null}
      destroyOnHidden={true}
    >
      {loading ? (
        <PostLoadingCard />
      ) : (
        data && (
          <div style={{ overflowY: "scroll", maxHeight: "80vh" }}>
            <PostItem
              post={{ ...data, latestComment, score }}
              enableViewModal={false}
            />
            <CommentProvider>
              <CommentList postId={data.id} />
            </CommentProvider>
          </div>
        )
      )}
    </Modal>
  );
};

export default ViewPostModal;

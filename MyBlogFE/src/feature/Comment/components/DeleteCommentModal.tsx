import { commentApi } from "@/api";
import {
  useApiMutation,
  useFixInfiniteQuery,
  useSafeTranslation,
} from "@/hooks";
import type { GetCommentsData } from "@/types/comment.type";
import type { GetPostDetailData } from "@/types/post.type";
import { App, Button, Modal } from "antd";

type DeleteCommentModalProps = {
  id: string;
  parentCommentId?: string;
  postId: string;
  onClose: () => void;
};

export const DeleteCommentModal = ({
  id,
  postId,
  parentCommentId,
  onClose,
}: DeleteCommentModalProps) => {
  const { t } = useSafeTranslation();
  const { message } = App.useApp();
  const { removeItem } = useFixInfiniteQuery<GetCommentsData>({
    keySelector: (item) => item.id,
  });

  const { updateItem } = useFixInfiniteQuery<GetPostDetailData>({
    keySelector: (item) => item.id,
  });

  //   onSuccess: () => {
  //     message.success(t("DeletePostSuccess"));
  //     // onReturn(id);
  //     removePost(id);
  //     onClose();
  //   },
  //   onError: () => {
  //     message.error(t("DeletePostFailed"));
  //   },
  // });

  const { isLoading: loading, mutate: execute } = useApiMutation<void, string>({
    mutationKey: ["deletePost", id],
    mutationFn: commentApi.deleteComment,
    onSuccess: () => {
      message.success(t("DeleteCommentSuccess"));
      removeItem(["getPostComments", postId], id);
      parentCommentId && removeItem(["getChildComments", parentCommentId], id);

      updateItem(["posts"], postId, (old) => {
        if (!old) return old;
        return {
          ...old,
          commentCount: old.commentCount - 1,
        };
      });
      onClose();
    },
    onError: () => {
      message.error(t("DeleteCommentFailed"));
    },
  });
  return (
    <Modal
      open={true}
      onCancel={onClose}
      footer={[
        <Button key="back" onClick={() => onClose()}>
          {t("Cancel")}
        </Button>,
        <Button
          key="submit"
          type="primary"
          danger
          onClick={() => execute(id)}
          loading={loading}
        >
          {t("Delete")}
        </Button>,
      ]}
    >
      <p>{t("DeleteCommentConfirm")}</p>
    </Modal>
  );
};

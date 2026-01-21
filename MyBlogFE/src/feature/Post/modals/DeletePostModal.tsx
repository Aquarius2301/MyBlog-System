import { postApi } from "@/api";
import {
  useApiMutation,
  useFixInfiniteQuery,
  useSafeTranslation,
} from "@/hooks";
import { App, Button, Modal } from "antd";

type DeletePostModalProps = {
  id: string;
  isModalOpen: boolean;
  onClose: () => void;
};

const DeletePostModal = ({
  id,
  isModalOpen,
  onClose,
}: DeletePostModalProps) => {
  const { t } = useSafeTranslation();
  const { message } = App.useApp();
  const { removeItem } = useFixInfiniteQuery<{ id: string }>({
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
    mutationFn: postApi.deletePost,
    onSuccess: () => {
      message.success(t("DeletePostSuccess"));

      removeItem(["posts"], id);
      onClose();
    },
    onError: () => {
      message.error(t("DeletePostFailed"));
    },
  });

  return (
    <Modal
      open={isModalOpen}
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
      <p>{t("DeletePostConfirm")}</p>
    </Modal>
  );
};

export default DeletePostModal;

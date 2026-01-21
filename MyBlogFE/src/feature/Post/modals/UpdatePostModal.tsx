import { postApi } from "@/api";
import {
  ImageUpload,
  ImageUploadProgress,
  useImageUpload,
} from "@/feature/UploadImage";
import {
  useApiMutation,
  useApiQuery,
  useFixInfiniteQuery,
  useSafeTranslation,
} from "@/hooks";
import type { GetPostDetailData, UpdatePostRequest } from "@/types/post.type";
import { Button, Form, Input, Modal, Spin, App, type UploadFile } from "antd";
import { useEffect, useState } from "react";

type UpdatePostModalProps = {
  id: string;
  link: string;
  isModalOpen: boolean;
  onClose: () => void;
};

export const UpdatePostModal = ({
  id,
  link,
  isModalOpen,
  onClose,
}: UpdatePostModalProps) => {
  const [files, setFiles] = useState<UploadFile[]>([]);
  const { loading, loadingContent, loadingPercent, uploadImages } =
    useImageUpload();
  const [form] = Form.useForm();
  const { t } = useSafeTranslation();
  const { message } = App.useApp();
  const { updateItem } = useFixInfiniteQuery<GetPostDetailData>({
    keySelector: (item) => item.id,
  });

  const { mutate } = useApiMutation<GetPostDetailData>({
    mutationKey: ["updatePost", id],
    mutationFn: ({ request, id }: { request: UpdatePostRequest; id: string }) =>
      postApi.updatePost(request, id),
    onSuccess: (data) => {
      updateItem(["posts"], id, () => data!);
      message.success(t("UpdatePostSuccess"));
      onClose();
    },
  });

  const {
    data,
    isLoading: apiLoading,
    refetch,
  } = useApiQuery<GetPostDetailData>({
    queryKey: ["getPostByLink", link],
    queryFn: () => postApi.getPostByLink(link),
    enabled: false,
  });

  /** ===== Load old data===== */
  useEffect(() => {
    if (!isModalOpen || !data) return;

    form.setFieldsValue({ content: data?.content });

    setFiles(
      data?.postPictures?.map((url, index) => ({
        uid: `${index}`,
        name: `image-${index}.jpg`,
        status: "done",
        url,
      })) ?? []
    );
  }, [isModalOpen, data]);

  useEffect(() => {
    if (isModalOpen) {
      refetch();
    }
  }, [isModalOpen]);

  /** ===== Submit Update ===== */
  const handleSubmit = async (values: UpdatePostRequest) => {
    const pictureRes = await uploadImages(files);
    if (!pictureRes) {
      message.error("UploadFailed");
      return;
    }

    const request = {
      content: values.content,
      pictures: pictureRes,
    };

    mutate({ request, id });
  };

  return (
    <Modal
      title={t("UpdatePost")}
      open={isModalOpen}
      footer={[
        <Button key="cancel" onClick={onClose} disabled={loading}>
          {t("Cancel")}
        </Button>,
        <Button
          key="button"
          type="primary"
          htmlType="submit"
          loading={loading}
          onClick={() => form.submit()}
        >
          {t("UpdatePost")}
        </Button>,
      ]}
      onCancel={() => {
        !loading ? onClose && onClose() : null;
      }}
      width={1000}
    >
      {apiLoading ? (
        <Spin />
      ) : (
        <Form form={form} onFinish={handleSubmit} disabled={loading}>
          <Form.Item
            label="Content"
            name="content"
            rules={[{ required: true, message: t("ContentEmpty") }]}
          >
            <Input.TextArea rows={6} placeholder={t("ContentPlaceholder")} />
          </Form.Item>

          <Form.Item label="Pictures">
            <ImageUpload
              files={files}
              setFiles={(newFiles) => {
                setFiles(newFiles);
              }}
              disabled={loading}
            />
          </Form.Item>

          <ImageUploadProgress
            loading={loading}
            loadingContent={loadingContent}
            loadingPercent={loadingPercent}
          />
        </Form>
      )}
    </Modal>
  );
};

export default UpdatePostModal;

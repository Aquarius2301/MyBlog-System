import { postApi } from "@/api";
import {
  ImageUpload,
  ImageUploadProgress,
  useImageUpload,
} from "@/feature/UploadImage";
import {
  useApiMutation,
  useFixInfiniteQuery,
  useSafeTranslation,
} from "@/hooks";
import type { CreatePostRequest, GetPostDetailData } from "@/types/post.type";
import { App, Button, Form, Input, Modal, type UploadFile } from "antd";

import { useState } from "react";

type CreatePostModalProps = {
  modalVisible: boolean;
  onClose: () => void;
  // onReturn: (data: any) => void;
};

const CreatePostModal = ({
  modalVisible,
  onClose,
}: // onReturn,
CreatePostModalProps) => {
  const [files, setFiles] = useState<UploadFile[]>([]);
  const [form] = Form.useForm();
  const { t, tUnsafe } = useSafeTranslation();
  const { message } = App.useApp();
  // const { addPost } = usePost();
  const { addItem } = useFixInfiniteQuery<GetPostDetailData>({
    keySelector: (item) => item.id,
  });

  const { mutate } = useApiMutation<GetPostDetailData, CreatePostRequest>({
    mutationKey: ["createPost"],
    mutationFn: postApi.createPost,
    onSuccess: (data) => {
      addItem(["posts"], data!);
      message.success(t("CreatePostSuccess"));
      onClose();
    },
  });

  const { loading, loadingContent, loadingPercent, uploadImages } =
    useImageUpload();

  const handleSubmit = async (values: { content: string }) => {
    const pictureRes = await uploadImages(files);

    console.log("pictureRes", pictureRes);
    if (pictureRes === null) {
      message.error("UploadFailed");
      return;
    }

    const request: CreatePostRequest = {
      content: values.content,
      pictures: pictureRes,
    };

    !loading && mutate(request);

    // const res = await postApi.createPost(request);

    // if (res.statusCode === 201 && res.data) {
    //   addPost(res.data);
    //   message.success(t("CreatePostSuccess"));
    //   onClose();
    // }
  };

  return (
    <>
      <Modal
        open={modalVisible}
        title={t("CreatePost")}
        footer={[
          <Button
            key="cancel"
            onClick={() => {
              onClose && onClose();
            }}
            disabled={loading}
          >
            {t("Cancel")}
          </Button>,
          <Button
            key="create"
            type="primary"
            onClick={() => form.submit()}
            loading={loading}
          >
            {t("CreatePost")}
          </Button>,
        ]}
        onCancel={() => {
          !loading ? onClose && onClose() : null;
        }}
        destroyOnHidden
        width={1000}
      >
        <Form form={form} onFinish={handleSubmit} disabled={loading}>
          <Form.Item
            label={t("Content")}
            name="content"
            rules={[{ required: true, message: t("ContentEmpty") }]}
          >
            <Input.TextArea rows={6} placeholder={t("ContentPlaceholder")} />
          </Form.Item>

          <Form.Item name="pictures" label={t("Pictures")}>
            <ImageUpload files={files} setFiles={setFiles} disabled={loading} />
          </Form.Item>

          <ImageUploadProgress
            loading={loading}
            loadingContent={loadingContent}
            loadingPercent={loadingPercent}
          />
        </Form>
      </Modal>

      {/* {loading && <Spin fullscreen percent={loadingPercent} />} */}
    </>
  );
};

export default CreatePostModal;

import { App, Button, Card, Flex, Form, Modal } from "antd";
import TextArea from "antd/es/input/TextArea";
import { useEffect, useRef, useState } from "react";
import { SendOutlined } from "@ant-design/icons";
import {
  ImageUpload,
  ImageUploadProgress,
  useImageUpload,
} from "@/feature/UploadImage/UploadImage";
import type {
  GetCommentsData,
  UpdateCommentRequest,
} from "@/types/comment.type";
import type { UploadFile } from "antd";
import {
  useApiMutation,
  useApiQuery,
  useFixInfiniteQuery,
  useSafeTranslation,
} from "@/hooks";
import { commentApi } from "@/api";
import { useQueryClient } from "@tanstack/react-query";

type CommentCreateInputProps = {
  id: string;
  postId?: string;
  parentCommentId?: string;
  onClose?: () => void;
};

const CommentUpdateModal = ({
  id,
  postId,
  onClose,
  parentCommentId,
}: CommentCreateInputProps) => {
  const [commentContent, setCommentContent] = useState("");
  const [files, setFiles] = useState<UploadFile[]>([]);
  const [form] = Form.useForm();
  const textareaRef = useRef<any>(null);
  const { message } = App.useApp();
  const { t } = useSafeTranslation();

  const queryClient = useQueryClient();

  const { data, isLoading } = useApiQuery<GetCommentsData>({
    queryKey: ["getCommentById", id],
    queryFn: () => commentApi.getCommentById(id),
    enabled: true,
  });

  const { updateItem: updateCommentItem } =
    useFixInfiniteQuery<GetCommentsData>({
      keySelector: (item) => item.id,
    });

  const { isLoading: updateLoading, mutate: updateExecute } = useApiMutation<
    GetCommentsData,
    UpdateCommentRequest
  >({
    mutationKey: ["updateComment", id],
    mutationFn: (data) => commentApi.updateComment(data, id),

    onSuccess: (data) => {
      if (!data) return;

      message.success(t("CommentUpdated"));

      if (parentCommentId) {
        // update child comment
        updateCommentItem(
          ["getChildComments", parentCommentId],
          data.id,
          () => data,
        );
      } else if (postId) {
        // update post comment
        updateCommentItem(["getPostComments", postId], data.id, () => data);
      }

      queryClient.resetQueries({ queryKey: ["getCommentById"] });

      setCommentContent("");
      setFiles([]);

      onClose && onClose();
    },

    onError: () => {
      message.error(t("CommentUpdatedFailed"));
    },
  });

  const { loading, loadingContent, loadingPercent, uploadImages } =
    useImageUpload();

  const handleUpdate = async () => {
    const pictureUrls = await uploadImages(files);

    if (pictureUrls === null) {
      message.error(t("CommentUpdatedFailed"));
      return;
    }

    updateExecute({
      content: commentContent,
      pictures: pictureUrls || [],
    });
  };

  useEffect(() => {
    if (!id || !data) return;

    setCommentContent(data.content);

    form.setFieldsValue({ content: data.content });

    setFiles(
      data?.pictures?.map((url, index) => ({
        uid: `${index}`,
        name: `image-${index}.jpg`,
        status: "done",
        url,
      })) ?? [],
    );
  }, [id, data]);

  return (
    <Modal open={true} onCancel={onClose} footer={null}>
      <Card>
        <Form
          disabled={updateLoading || loading}
          onFinish={handleUpdate}
          form={form}
        >
          <Form.Item>
            {/* Input + Button */}
            <Flex gap={8} align="flex-end">
              <TextArea
                ref={textareaRef}
                // placeholder={
                //   comment
                //     ? `${t("ReplyTo")} ${comment.commenter?.displayName}...`
                //     : t("AddComment")
                // }
                autoSize={{ minRows: 1, maxRows: 4 }}
                value={commentContent}
                onChange={(e) => setCommentContent(e.target.value)}
                onPressEnter={(e) => {
                  if (e.shiftKey) return; // Allow Shift+Enter for new line
                  e.preventDefault();
                  if (commentContent.trim() || files.length > 0) {
                    form.submit();
                  }
                }}
              />

              <Button
                type="primary"
                icon={<SendOutlined />}
                loading={updateLoading || loading}
                disabled={!commentContent.trim() && files.length === 0}
                onClick={form.submit}
              />
            </Flex>
          </Form.Item>

          <Form.Item name="pictures">
            <ImageUpload
              files={files}
              setFiles={setFiles}
              disabled={updateLoading || loading}
            />
          </Form.Item>

          <ImageUploadProgress
            loading={loading}
            loadingContent={loadingContent}
            loadingPercent={loadingPercent}
          />
        </Form>
      </Card>
    </Modal>
  );
};

export default CommentUpdateModal;

import { App, Button, Card, Flex, Form } from "antd";
import TextArea from "antd/es/input/TextArea";
import { useRef, useState } from "react";
import { SendOutlined } from "@ant-design/icons";
import {
  ImageUpload,
  ImageUploadProgress,
  useImageUpload,
} from "@/feature/UploadImage/UploadImage";
import type {
  CreateCommentRequest,
  GetCommentsData,
} from "@/types/comment.type";
import type { UploadFile } from "antd";
import {
  useApiMutation,
  useFixInfiniteQuery,
  useSafeTranslation,
} from "@/hooks";
import { commentApi } from "@/api";
import type { GetPostDetailData } from "@/types/post.type";

type CommentCreateInputProps = {
  comment: GetCommentsData | null;
  postId?: string;
  parentCommentId?: string;
};

const CommentCreateInput = ({
  comment,
  postId,

  parentCommentId,
}: CommentCreateInputProps) => {
  const [commentContent, setCommentContent] = useState("");
  const [files, setFiles] = useState<UploadFile[]>([]);
  const [form] = Form.useForm();
  const textareaRef = useRef<any>(null);
  const { message } = App.useApp();
  const { t } = useSafeTranslation();

  const { addItem } = useFixInfiniteQuery<GetCommentsData>({
    keySelector: (item) => item.id,
  });

  const { updateItem } = useFixInfiniteQuery<GetPostDetailData>({
    keySelector: (item) => item.id,
  });

  const { isLoading: apiLoading, mutate: execute } = useApiMutation<
    GetCommentsData,
    CreateCommentRequest
  >({
    mutationKey: ["addComment", postId, parentCommentId],
    mutationFn: commentApi.addComment,
    onSuccess: (data) => {
      if (data) {
        message.success(t("CommentAdded"));
        if (parentCommentId) {
          addItem(["getChildComments", parentCommentId], data, "start");
          updateItem(["getPostComments", postId], data.id, (old) => {
            if (!old) return old;
            return {
              ...old,
              commentCount: old.commentCount + 1,
            };
          });
        } else if (postId) {
          addItem(["getPostComments", postId], data, "start");
        }
        updateItem(["posts"], postId, (old) => {
          if (!old) return old;
          return {
            ...old,
            commentCount: old.commentCount + 1,
          };
        });
        // updateItem(["getPostByLink", link], postId, (old) => {
        //   if (!old) return old;
        //   return {
        //     ...old,
        //     commentCount: old.commentCount + 1,
        //   };
        // });
      }
      setCommentContent("");
      setFiles([]);
    },
    onError: () => {
      message.error(t("CommentAddedFailed"));
    },
  });

  const { loading, loadingContent, loadingPercent, uploadImages } =
    useImageUpload();

  const handleSubmit = async () => {
    const pictureUrls = await uploadImages(files);

    if (pictureUrls === null) {
      message.error(t("CommentAddedFailed"));
    }
    const request = {
      postId: postId,
      parentCommentId: parentCommentId ?? null,
      content: commentContent,
      pictures: pictureUrls || [],
      replyAccountId: comment?.commenter?.id,
    } as CreateCommentRequest;

    execute(request);
  };

  return (
    <Card>
      <Form
        disabled={apiLoading || loading}
        onFinish={handleSubmit}
        form={form}
      >
        <Form.Item>
          {/* Input + Button */}
          <Flex gap={8} align="flex-end">
            <TextArea
              ref={textareaRef}
              placeholder={
                comment
                  ? `${t("ReplyTo")} ${comment.commenter?.displayName}...`
                  : t("AddComment")
              }
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
              loading={apiLoading || loading}
              disabled={!commentContent.trim() && files.length === 0}
              onClick={form.submit}
            />
          </Flex>
        </Form.Item>

        <Form.Item name="pictures">
          <ImageUpload
            files={files}
            setFiles={setFiles}
            disabled={apiLoading || loading}
          />
        </Form.Item>

        <ImageUploadProgress
          loading={loading}
          loadingContent={loadingContent}
          loadingPercent={loadingPercent}
        />
      </Form>
    </Card>
  );
};

export default CommentCreateInput;

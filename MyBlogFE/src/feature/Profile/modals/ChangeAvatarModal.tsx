import { Button, Form, Modal, Upload, App } from "antd";
import { useEffect, useState } from "react";
import type { UploadFile } from "antd";
import imageCompression from "browser-image-compression";
import { accountApi, uploadApi } from "@/api";
import { Text } from "@/components";
import { useApiMutation, useAuth, useSafeTranslation } from "@/hooks";
import { AVATAR_MAX_SIZE, AVATAR_MAX_SIZE_MB } from "@/constants";
import "./styles.css";

// Types
type FormValues = {
  pictures: string;
};

export type ChangeAvatarModalProps = {
  visible: boolean;
  onClose?: () => void;
  oldAvatarUrl?: string;
};

// Constants
const INITIAL_FILE_UID = "0";
const INITIAL_FILE_NAME = "oldAvatar.jpg";

export const ChangeAvatarModal = ({
  oldAvatarUrl,
  visible,
  onClose,
}: ChangeAvatarModalProps) => {
  // Hooks
  const [form] = Form.useForm<FormValues>();
  const { message } = App.useApp();
  const { t } = useSafeTranslation();
  const { changeAvatar } = useAuth();

  const [fileList, setFileList] = useState<UploadFile[]>([]);
  const [reviewUrl, setReviewUrl] = useState<string>("");

  // const {
  //   loading: uploadLoading,
  //   execute: executeUpload,
  //   reset: resetUpload,
  // } = useApi(uploadApi.upload, {
  //   onSuccess: async ({ data }) => {
  //     resetUpload();
  //     await updateAvatar(data[0]);
  //   },
  //   onError: ({ error }) => {
  //     message.error(error || t("ChangeAvatarFailed"));
  //   },
  // });
  const { isLoading: uploadLoading, mutate: executeUpload } = useApiMutation<
    string[],
    FormData
  >({
    mutationKey: ["uploadAvatar"],
    mutationFn: uploadApi.upload,
    onSuccess: async (data) => {
      data && (await updateAvatar(data[0]));
    },
    onError: (error) => {
      error.data && message.error(error.data || t("ChangeAvatarFailed"));
    },
  });

  // const { loading: submitLoading, execute: executeChangeAvatar } = useApi(
  //   accountApi.changeAvatar,
  //   {
  //     onSuccess: () => {
  //       message.success(t("AvatarChanged"));
  //       changeAvatar(reviewUrl);
  //       onClose?.();
  //     },
  //     onError: ({ error }) => {
  //       message.error(error || t("ChangeAvatarFailed"));
  //     },
  //   }
  // );

  const { isLoading: submitLoading, mutate: executeChangeAvatar } =
    useApiMutation<void, string>({
      mutationKey: ["changeAvatar"],
      mutationFn: accountApi.changeAvatar,
      onSuccess: () => {
        message.success(t("AvatarChanged"));
        changeAvatar(reviewUrl);
        onClose?.();
      },
      onError: (error) => {
        error.data && message.error(error.data || t("ChangeAvatarFailed"));
      },
    });

  //

  const isLoading = uploadLoading || submitLoading;

  // Initialize form with old avatar
  useEffect(() => {
    if (!visible || !oldAvatarUrl) return;

    form.setFieldValue("pictures", oldAvatarUrl);

    setFileList([
      {
        uid: INITIAL_FILE_UID,
        name: INITIAL_FILE_NAME,
        status: "done",
        url: oldAvatarUrl,
      },
    ]);
    setReviewUrl(oldAvatarUrl);
  }, [oldAvatarUrl, visible]);

  // Handlers
  const compressImageIfNeeded = async (file: File): Promise<File | Blob> => {
    if (file.size < AVATAR_MAX_SIZE) {
      return file;
    }

    return await imageCompression(file, {
      maxSizeMB: AVATAR_MAX_SIZE_MB,
    });
  };

  const updateAvatar = async (pictureUrl: string) => {
    setReviewUrl(pictureUrl);
    executeChangeAvatar(pictureUrl);
  };

  const handleUpload = async () => {
    const currentFile = fileList[0];

    if (!currentFile) return;

    // If the file is already uploaded (has URL), use it directly
    if (currentFile.url && !currentFile.originFileObj) {
      await updateAvatar(currentFile.url);
      return;
    }

    // Upload new file
    if (currentFile.originFileObj) {
      const formData = new FormData();
      const processedFile = await compressImageIfNeeded(
        currentFile.originFileObj
      );
      formData.append("Pictures", processedFile);
      await executeUpload(formData);
    }
  };

  const handleFileChange = ({
    fileList: newFileList,
  }: {
    fileList: UploadFile[];
  }) => {
    setFileList(newFileList.slice(-1));

    const reader = new FileReader();
    reader.addEventListener("load", () =>
      setReviewUrl(reader.result as string)
    );
    reader.readAsDataURL(newFileList[0].originFileObj as File);
  };

  const handleFileRemove = () => {
    setFileList([]);
  };

  // Computed values
  const isUploadDisabled =
    fileList.length === 0 ||
    (fileList[0]?.url !== undefined && !fileList[0]?.originFileObj) ||
    isLoading;

  return (
    <Modal
      open={visible}
      title={t("ChangeAvatar")}
      onCancel={onClose}
      destroyOnHidden
      footer={[
        <Button key="cancel" onClick={onClose} disabled={isLoading}>
          {t("Cancel")}
        </Button>,
        <Button
          key="submit"
          type="primary"
          onClick={form.submit}
          loading={isLoading}
          disabled={isUploadDisabled}
        >
          {t("UploadAvatar")}
        </Button>,
      ]}
    >
      <Form form={form} onFinish={handleUpload}>
        <Form.Item<FormValues> name="pictures" label={t("Avatar")}>
          <div className="avatar">
            {/* {fileList[0] && (
             
            )} */}
            <Upload
              id="pictures"
              disabled={isLoading}
              listType="picture-circle"
              beforeUpload={() => false}
              onChange={handleFileChange}
              onRemove={handleFileRemove}
              maxCount={1}
              accept="image/*"
              fileList={fileList}
              onDownload={() => false}
              onPreview={() => false}
              style={{ cursor: "pointer" }}
              showUploadList={false}
            >
              {reviewUrl ? (
                <img
                  src={reviewUrl}
                  alt="Avatar preview"
                  style={{
                    width: "100%",
                    height: "100%",
                    objectFit: "cover",
                    objectPosition: "center",
                    borderRadius: "50%",
                  }}
                />
              ) : (
                `+ ${t("SelectAvatar")}`
              )}
            </Upload>
          </div>
        </Form.Item>
        <div style={{ color: "gray", marginTop: "8px" }}>
          {t("ImageTooLarge1")} {AVATAR_MAX_SIZE_MB}
          {t("ImageTooLarge2")}
        </div>

        {isLoading && (
          <Text fontSize="small" style={{ marginBottom: "8px", color: "gray" }}>
            {t("UploadingAvatar")}
          </Text>
        )}
      </Form>
    </Modal>
  );
};

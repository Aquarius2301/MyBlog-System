import { Text } from "@/components";
import { useApiMutation, useSafeTranslation } from "@/hooks";
import { uploadApi } from "@/api";
import { POST_PICTURE_MAX_SIZE, POST_PICTURE_MAX_SIZE_MB } from "@/constants";
import { convertToMB } from "@/utils";
import { Progress, Upload, type UploadFile } from "antd";
import imageCompression from "browser-image-compression";
import { useState } from "react";

type ImageUploadProps = {
  files: UploadFile[];
  setFiles: (files: UploadFile[]) => void;
  disabled?: boolean;
  onUploadComplete?: (urls: string[]) => void;
  onUploadError?: () => void;
};

export const ImageUpload = ({
  files,
  setFiles,
  disabled = false,
}: ImageUploadProps) => {
  const { t } = useSafeTranslation();

  return (
    <>
      <Upload
        listType="picture-card"
        beforeUpload={() => false}
        fileList={files}
        onChange={({ fileList }) => setFiles(fileList)}
        multiple
        disabled={disabled}
      >
        + {t("AddPicture")}
      </Upload>
      <div style={{ color: "gray", marginTop: "8px" }}>
        {t("ImageTooLarge1")} {POST_PICTURE_MAX_SIZE_MB}
        {t("ImageTooLarge2")}
      </div>
    </>
  );
};

type UseImageUploadReturn = {
  loading: boolean;
  loadingContent: string;
  loadingPercent: number;
  uploadImages: (files: UploadFile[]) => Promise<string[] | null>;
};

export const useImageUpload = (): UseImageUploadReturn => {
  const [loading, setLoading] = useState(false);
  const [loadingContent, setLoadingContent] = useState("");
  const [loadingPercent, setLoadingPercent] = useState(0);
  const { t } = useSafeTranslation();

  const { mutateAsync } = useApiMutation({
    mutationKey: ["upload"],
    mutationFn: uploadApi.upload,
  });

  // const handleSuccess = (data: string[]) => {
  //   setLoadingPercent((prev) => prev + percentPerBatch);
  //   urls = urls.concat(data);
  // };

  const uploadImages = async (
    files: UploadFile[]
  ): Promise<string[] | null> => {
    let urls: string[] = [];
    const totalUploadPercentage = 95;
    const totalBatches = Math.ceil(files.length / 3);
    const percentPerBatch = totalUploadPercentage / totalBatches;

    try {
      setLoading(true);
      setLoadingPercent(0);

      for (let i = 0; i < files.length; i += 3) {
        const batch = files.slice(i, i + 3);
        const formData = new FormData();

        for (const file of batch) {
          if (file.originFileObj) {
            // File mới cần upload
            if (file.originFileObj.size <= POST_PICTURE_MAX_SIZE) {
              console.log("Not compressing image...", file.name);
              formData.append("Pictures", file.originFileObj);
            } else {
              console.log("Compressing image...", file.name);
              setLoadingContent(
                t("Compressing") +
                  file.name +
                  " (" +
                  convertToMB(file.originFileObj.size) +
                  ") ..."
              );
              const compressedFile = await imageCompression(
                file.originFileObj,
                {
                  maxSizeMB: POST_PICTURE_MAX_SIZE_MB,
                }
              );
              formData.append("Pictures", compressedFile);
            }
          } else if (file.url) {
            urls.push(file.url);
          }
        }

        if (formData.has("Pictures")) {
          setLoadingContent(
            t("Uploading") +
              batch
                .filter((f) => f.originFileObj)
                .map((file) => file.name)
                .join(", ") +
              " (" +
              convertToMB(
                batch.reduce(
                  (acc, file) =>
                    acc + (file.originFileObj ? file.originFileObj.size : 0),
                  0
                )
              ) +
              ") ..."
          );

          console.log(formData.getAll("Pictures"));
          // const res = await uploadApi.upload(formData);

          const res = await mutateAsync(formData);

          if (res.data) {
            setLoadingPercent((prev) => prev + percentPerBatch);
            urls = urls.concat(res.data!);
          } else {
            throw new Error("UploadFailed");
          }
          // if (res.statusCode === 200 && res.data) {
          //   setLoadingPercent((prev) => prev + percentPerBatch);
          //   urls = urls.concat(res.data);
          // } else {
          //   throw new Error("UploadFailed");
          // }
        }
      }

      setLoading(false);
      setLoadingPercent(0);
      return urls;
    } catch (error) {
      console.error("Upload error:", error);
      setLoading(false);
      setLoadingPercent(0);
      return null;
    }
  };

  return {
    loading,
    loadingContent,
    loadingPercent,
    uploadImages,
  };
};

type ImageUploadProgressProps = {
  loading: boolean;
  loadingContent: string;
  loadingPercent: number;
};

export const ImageUploadProgress = ({
  loading,
  loadingContent,
  loadingPercent,
}: ImageUploadProgressProps) => {
  if (!loading) return null;

  return (
    <>
      <Text fontSize="small" style={{ marginBottom: "8px", color: "gray" }}>
        {loadingContent}
      </Text>
      <Progress percent={Math.floor(loadingPercent)} showInfo />
    </>
  );
};

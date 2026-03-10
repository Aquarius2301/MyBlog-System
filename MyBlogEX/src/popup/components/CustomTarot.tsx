import type { CustomTarotRequest } from "@/types/tarot.type";
import { Divider, Form } from "antd";
import TextArea from "antd/es/input/TextArea";
import { Button } from "antd";
import { SendOutlined } from "@ant-design/icons";
import useSafeTranslation from "@/hooks/useSafeTranslation";
import tarotApi from "@/api/tarot.api";
import useApiMutation from "@/hooks/useApiMutation";

declare const t: any;

type CustomTarotProps = {
  isActive: boolean;
  onReturn: (data: string) => void;
};

const CustomTarot = ({ isActive, onReturn }: CustomTarotProps) => {
  const { t } = useSafeTranslation();
  const [form] = Form.useForm();

  const { mutate, isLoading } = useApiMutation<string, CustomTarotRequest>({
    mutationKey: ["tarot-reading"],
    mutationFn: tarotApi.getCustomTarotReading,
    onSuccess: (data) => {
      data && onReturn(data);
    },
  });

  const getLanguage = () => {
    var lang = localStorage.getItem("i18nextLng") || "en";
    switch (lang) {
      case "ja":
        return "Japanese";
      case "vi":
        return "Vietnamese";
      default:
        return "English";
    }
  };

  const onFinish = (values: CustomTarotRequest) => {
    mutate({
      question: values.question,
      language: getLanguage(),
    });
  };

  return (
    <div style={{ marginTop: 10 }}>
      <Form
        form={form}
        layout="vertical"
        onFinish={onFinish}
        disabled={isLoading}
      >
        <Form.Item
          name="question"
          label={t("TarotEnterYourQuestion")}
          rules={[
            { required: isActive, message: t("TarotPleaseEnterQuestionEmpty") },
          ]}
        >
          <TextArea
            rows={4}
            placeholder={t("TarotEnterYourQuestionPlaceholder")}
            maxLength={500}
            showCount
          />
        </Form.Item>
        <Divider />
        <Form.Item>
          <Button
            loading={isLoading}
            disabled={isLoading}
            type="primary"
            htmlType="submit"
            size="large"
            block
            icon={<SendOutlined />}
            style={{
              height: 45,
              borderRadius: 8,
              background: "linear-gradient(to right, #722ed1, #2f54eb)",
              border: "none",
            }}
          >
            {t("TarotDrawButton")}
          </Button>
        </Form.Item>
      </Form>
    </div>
  );
};

export default CustomTarot;

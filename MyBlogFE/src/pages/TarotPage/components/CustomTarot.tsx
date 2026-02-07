import { useApiMutation, useSafeTranslation } from "@/hooks";
import type { CustomTarotRequest } from "@/types/tarot.type";
import { Divider, Form } from "antd";
import TextArea from "antd/es/input/TextArea";
import { Button } from "antd";
import { SendOutlined } from "@ant-design/icons";
import { tarotApi } from "@/api";

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
        disabled={isLoading}
        form={form}
        layout="vertical"
        onFinish={onFinish}
      >
        <Form.Item<CustomTarotRequest>
          name="question"
          label={t("TarotEnterYourQuestion")}
          rules={[
            {
              required: isActive === true,
              message: t("TarotPleaseEnterQuestionEmpty"),
            },
          ]}
        >
          <TextArea
            rows={6}
            placeholder={t("TarotEnterYourQuestionPlaceholder")}
            maxLength={500}
            showCount
          />
        </Form.Item>

        <Divider />

        <Form.Item>
          <Button
            disabled={isLoading}
            loading={isLoading}
            type="primary"
            htmlType="submit"
            size="large"
            block
            icon={<SendOutlined />}
            style={{
              height: 50,
              borderRadius: 8,
              background: "linear-gradient(to r, #722ed1, #2f54eb)",
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

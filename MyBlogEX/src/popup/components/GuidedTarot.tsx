import type { GuidedTarotRequest } from "@/types/tarot.type";
import { Button, Divider, Form, Space } from "antd";
import Select from "antd/es/select";
import { useForm } from "antd/es/form/Form";
import { SendOutlined } from "@ant-design/icons";
import useSafeTranslation from "@/hooks/useSafeTranslation";
import useApiMutation from "@/hooks/useApiMutation";
import tarotApi from "@/api/tarot.api";

type GuidedTarotProps = {
  isActive: boolean;
  onReturn: (data: string) => void;
};

const GuidedTarot = ({ isActive, onReturn }: GuidedTarotProps) => {
  const { t } = useSafeTranslation();
  const [form] = useForm();

  const { mutate, isLoading } = useApiMutation<string, GuidedTarotRequest>({
    mutationKey: ["tarot-reading"],
    mutationFn: tarotApi.getGuidedTarotReading,
    onSuccess: (data) => {
      data && onReturn(data);
    },
  });

  const getNumberOfCards = (time: string) => {
    switch (time) {
      case "today":
        return 1;
      case "1_week":
        return 3;
      case "1_month":
        return 3;
      case "3_months":
        return 5;
      case "6_months":
        return 6;
      case "current_year":
        return 12;
      default:
        return 3;
    }
  };

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

  const onFinish = (values: GuidedTarotRequest) => {
    mutate({
      numberOfCards: getNumberOfCards(values.time || "today"),
      spreadType: values.spreadType || "",
      time: values.time || "",
      language: getLanguage(),
    });
  };

  return (
    <Space direction="vertical" style={{ width: "100%", marginTop: 10 }}>
      <Form
        form={form}
        layout="vertical"
        onFinish={onFinish}
        disabled={isLoading}
      >
        <Form.Item
          name="spreadType"
          label={t("TarotSelectCategory")}
          rules={[
            { required: isActive, message: t("TarotSelectCategoryEmpty") },
          ]}
        >
          <Select size="large" placeholder={t("TarotSelectCategory")}>
            <Select.Option value="love">{t("TarotLove")}</Select.Option>
            <Select.Option value="career">{t("TarotCareer")}</Select.Option>
            <Select.Option value="money">{t("TarotMoney")}</Select.Option>
            <Select.Option value="general">{t("TarotGeneral")}</Select.Option>
          </Select>
        </Form.Item>

        <Form.Item
          name="time"
          label={t("TarotSelectTimeframe")}
          rules={[
            { required: isActive, message: t("TarotSelectTimeframeEmpty") },
          ]}
        >
          <Select size="large" placeholder={t("TarotSelectTimeframe")}>
            <Select.Option value="today">{t("TarotToday")}</Select.Option>
            <Select.Option value="1_week">{t("Tarot1Week")}</Select.Option>
            <Select.Option value="1_month">{t("Tarot1Month")}</Select.Option>
            <Select.Option value="3_months">{t("Tarot3Months")}</Select.Option>
          </Select>
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
    </Space>
  );
};

export default GuidedTarot;

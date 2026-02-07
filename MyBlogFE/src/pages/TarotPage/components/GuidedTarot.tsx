import { useApiMutation, useSafeTranslation } from "@/hooks";
import type { GuidedTarotRequest } from "@/types/tarot.type";
import { Button, Divider, Form, Space } from "antd";
import Select from "antd/es/select";
import { Option } from "antd/es/mentions";
import { useForm } from "antd/es/form/Form";
import { SendOutlined } from "@ant-design/icons";
import { tarotApi } from "@/api";

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
    <Space vertical style={{ width: "100%", marginTop: 10 }}>
      <Form
        disabled={isLoading}
        form={form}
        layout="vertical"
        onFinish={onFinish}
      >
        <Form.Item<GuidedTarotRequest>
          name="spreadType"
          label={t("TarotSelectCategory")}
          rules={[
            {
              required: isActive === true,
              message: t("TarotSelectCategoryEmpty"),
            },
          ]}
        >
          <Select size="large" placeholder={t("TarotSelectCategory")}>
            <Option value="love">{t("TarotLove")}</Option>
            <Option value="career">{t("TarotCareer")}</Option>
            <Option value="money">{t("TarotMoney")}</Option>
            <Option value="general">{t("TarotGeneral")}</Option>
          </Select>
        </Form.Item>
        <Form.Item<GuidedTarotRequest>
          name="time"
          label={t("TarotSelectTimeframe")}
          rules={[
            {
              required: isActive === true,
              message: t("TarotSelectTimeframeEmpty"),
            },
          ]}
        >
          <Select size="large" placeholder={t("TarotSelectTimeframe")}>
            <Option value="today">{t("TarotToday")}</Option>
            <Option value="1_week">{t("Tarot1Week")}</Option>
            <Option value="1_month">{t("Tarot1Month")}</Option>
            <Option value="3_months">{t("Tarot3Months")}</Option>
            <Option value="6_months">{t("Tarot6Months")}</Option>
            <Option value="current_year">{t("TarotCurrentYear")}</Option>
          </Select>
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
    </Space>
  );
};

export default GuidedTarot;

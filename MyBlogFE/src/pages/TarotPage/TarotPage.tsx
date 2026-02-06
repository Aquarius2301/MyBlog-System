import { useState } from "react";
import {
  Card,
  Tabs,
  Select,
  Input,
  Button,
  Space,
  Typography,
  Form,
  Divider,
} from "antd";
import {
  BulbOutlined,
  EditOutlined,
  SendOutlined,
  StarOutlined,
} from "@ant-design/icons";
import { PageLayout } from "@/components";
import { useApiMutation, useSafeTranslation } from "@/hooks";
import type { TarotReadingRequest } from "@/types/tarot.type";
import { tarotApi } from "@/api";

const { Title, Text } = Typography;
const { TextArea } = Input;
const { Option } = Select;

const TarotPage = () => {
  const [form] = Form.useForm();
  const [activeTab, setActiveTab] = useState("guided");
  const { t } = useSafeTranslation();

  const { mutate, data, isLoading } = useApiMutation<
    string,
    TarotReadingRequest
  >({
    mutationKey: ["tarot-reading"],
    mutationFn: tarotApi.getTarotReading,
    // onSuccess: (data) => {
    //   console.log("Kết quả rút bài:", data);
    // },
  });

  // Mutation gọi API rút bài
  //   const drawMutation = useMutation({
  //     mutationFn: async (values) => {
  //       // Logic xử lý nội dung câu hỏi dựa trên tab đang chọn
  //       let finalQuestion = "";
  //       if (activeTab === 'guided') {
  //         finalQuestion = `Xem về ${values.category} trong khoảng thời gian ${values.timeframe}.`;
  //       } else {
  //         finalQuestion = values.customQuestion;
  //       }

  //       console.log("Câu hỏi gửi đi:", finalQuestion);
  //       // return await tarotApi.draw({ question: finalQuestion });
  //     },
  //   });

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

  const onFinish = (values: TarotReadingRequest) => {
    if (activeTab === "guided") {
      mutate({
        numberOfCards: getNumberOfCards(values.time || "today"),
        spreadType: values.spreadType || "",
        time: values.time || "",
        language: getLanguage(),
        question: "",
      });
    } else {
      mutate({
        numberOfCards: 3,
        spreadType: null,
        time: null,
        question: values.question,
        language: getLanguage(),
      });
    }
  };

  // const dataa = () => {
  //   return `html\n<h1>Daily Career Reading: Navigating Change and Challenging Choices</h1>\n\n<p>Welcome to your daily career reading. Today's spread brings us the Wheel of Fortune (Upright), the Eight of Swords (Reversed), and The Lovers (Reversed). This combination suggests a day of significant shifts, a release from limitations, and the need for careful consideration of your path forward.</p>\n\n<h2>Card Meanings:</h2>\n\n<h3>The Wheel of Fortune (Upright):</h3>\n<p>The Wheel of Fortune in its upright position is a powerful indicator of change, cycles, and destiny. It signifies that things are in motion, and external forces are at play, guiding you towards new opportunities or a shift in circumstances. This card suggests that what is happening today is part of a larger cosmic plan, and while you may not have complete control, you are on the cusp of a significant turn. Embrace the fluidity of the situation, as it's often for your highest good, even if the changes are unexpected.</p>\n\n<h3>Eight of Swords (Reversed):</h3>\n<p>The Eight of Swords, typically representing feeling trapped or restricted, appears here in its reversed aspect. This is excellent news for your career! It signifies a breaking free from self-imposed limitations, old patterns, or external constraints that have been holding you back. You are starting to see the truth of your situation, realizing that the \"chains\" were often more psychological than real. This is a day to shed outdated beliefs about what you can and cannot achieve in your professional life.</p>\n\n<h3>The Lovers (Reversed):</h3>\n<p>The Lovers, in its reversed position, introduces a note of caution. This card typically signifies harmony, choices, and relationships. Reversed, it can indicate disharmony, difficult choices, conflicting desires, or a lack of alignment in your values or decisions. In a career context, this might mean facing a choice where the options are not clear-cut, or where your personal desires are at odds with professional obligations. It can also point to a breakdown in communication or a lack of commitment to a particular path.</p>\n\n<h2>Connecting the Cards:</h2>\n\n<p>The Wheel of Fortune sets the stage for a day of inevitable change. You are being moved by currents beyond your immediate control, and this movement is likely to be positive, opening up new possibilities. The Eight of Swords (Reversed) perfectly complements this by indicating that you are now in a position to actively embrace these changes. The limitations you felt are dissolving, allowing you to step into this new phase with more freedom. </p>\n\n<p>However, The Lovers (Reversed) acts as a vital counterbalance. While you are breaking free from old constraints, the day might present you with a significant career choice. This choice may be complex, requiring you to weigh different paths, values, and potential outcomes carefully. There might be a feeling of internal conflict or external pressure related to this decision. The reversed Lovers suggests that without careful consideration, you might make a choice that leads to disharmony or regret. This is not a time for impulsive decisions, especially when faced with significant crossroads.</p>\n\n<h2>Advice for Today:</h2>\n\n<ol>\n    <li><strong>Embrace the Momentum:</strong> The Wheel of Fortune is turning. Instead of resisting change, try to understand where it's leading you. Be open to new opportunities and unexpected turns. This is a day to flow with the rhythm of change.</li>\n    <li><strong>Recognize Your Freedom:</strong> The Eight of Swords (Reversed) is a powerful affirmation. Acknowledge that you are no longer as bound as you once thought. Identify any limiting beliefs or self-doubts that have been holding you back and actively release them. You have the power to move forward.</li>\n    <li><strong>Deliberate Your Choices:</strong> The Lovers (Reversed) warns against hasty decisions. If faced with a significant career choice today, take your time. Seek clarity by understanding your own motivations and values. It might be helpful to discuss your options with a trusted advisor or friend, but ultimately, the decision must align with your inner truth.</li>\n    <li><strong>Seek Inner Harmony:</strong> The reversed Lovers can also suggest a need to re-evaluate your alignment. Are your current career actions truly in line with your passions and goals? If not, this is a day to start the process of bringing them into better harmony.</li>\n    <li><strong>Communicate with Clarity:</strong> If your choices involve others, ensure clear and honest communication. Misunderstandings can arise, so be precise in expressing your thoughts and intentions.</li>\n</ol>\n\n<p>Today is a dynamic day in your career, marked by fortunate shifts and the breaking of old chains. However, navigate any immediate decisions with thoughtfulness and a focus on creating a future that resonates with your deepest values. Trust the process of change, but be discerning in the choices you make as a result.</p>\n`;
  // };

  return (
    <PageLayout title={t("TarotPage")}>
      <div style={{ maxWidth: 600, margin: "40px auto", padding: "0 20px" }}>
        <Card
          style={{
            borderRadius: 16,
            // boxShadow: "0 8px 24px rgba(149, 157, 165, 0.2)",
          }}
        >
          <div style={{ textAlign: "center", marginBottom: 24 }}>
            <Title level={2}>
              <StarOutlined style={{ color: "#722ed1", marginRight: 10 }} />

              {t("TarotTitle")}
            </Title>
            <Text type="secondary">{t("TarotCaption")}</Text>
          </div>

          <Form
            disabled={isLoading}
            form={form}
            layout="vertical"
            onFinish={onFinish}
            initialValues={{ category: "general", timeframe: "1_week" }}
          >
            <Tabs
              activeKey={activeTab}
              onChange={setActiveTab}
              centered
              items={[
                {
                  key: "guided",
                  label: (
                    <span>
                      <BulbOutlined /> {t("TarotGuided")}
                    </span>
                  ),
                  children: (
                    <Space vertical style={{ width: "100%", marginTop: 10 }}>
                      <Form.Item<TarotReadingRequest>
                        name="spreadType"
                        label={t("TarotSelectCategory")}
                        rules={[
                          {
                            required: activeTab === "guided",
                            message: t("TarotSelectCategoryEmpty"),
                          },
                        ]}
                      >
                        <Select
                          size="large"
                          placeholder={t("TarotSelectCategory")}
                        >
                          <Option value="love">{t("TarotLove")}</Option>
                          <Option value="career">{t("TarotCareer")}</Option>
                          <Option value="money">{t("TarotMoney")}</Option>
                          <Option value="general">{t("TarotGeneral")}</Option>
                        </Select>
                      </Form.Item>

                      <Form.Item<TarotReadingRequest>
                        name="time"
                        label={t("TarotSelectTimeframe")}
                        rules={[
                          {
                            required: activeTab === "guided",
                            message: t("TarotSelectTimeframeEmpty"),
                          },
                        ]}
                      >
                        <Select
                          size="large"
                          placeholder={t("TarotSelectTimeframe")}
                        >
                          <Option value="today">{t("TarotToday")}</Option>
                          <Option value="1_week">{t("Tarot1Week")}</Option>
                          <Option value="1_month">{t("Tarot1Month")}</Option>
                          <Option value="3_months">{t("Tarot3Months")}</Option>
                          <Option value="6_months">{t("Tarot6Months")}</Option>
                          <Option value="current_year">
                            {t("TarotCurrentYear")}
                          </Option>
                        </Select>
                      </Form.Item>
                    </Space>
                  ),
                },
                {
                  key: "custom",
                  label: (
                    <span>
                      <EditOutlined /> {t("TarotCustom")}
                    </span>
                  ),
                  children: (
                    <div style={{ marginTop: 10 }}>
                      <Form.Item<TarotReadingRequest>
                        name="question"
                        label={t("TarotEnterYourQuestion")}
                        rules={[
                          {
                            required: activeTab === "custom",
                            message: t("TarotPleaseEnterQuestionEmpty"),
                          },
                        ]}
                      >
                        <TextArea
                          rows={5}
                          placeholder={t("TarotEnterYourQuestionPlaceholder")}
                          maxLength={200}
                          showCount
                        />
                      </Form.Item>
                    </div>
                  ),
                },
              ]}
            />

            <Divider />

            <Form.Item>
              <Button
                disabled={isLoading}
                loading={isLoading}
                type="primary"
                htmlType="submit"
                size="large"
                block
                //   loading={drawMutation.isPending}
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
        </Card>
        {data && (
          <Card
            style={{
              marginTop: 24,
              borderRadius: 12,
              textAlign: "left", // Tarot nên để canh lề trái cho dễ đọc lời phán
              // border: "1px solid #d3adf7",
              // background: "#fffbfe",
            }}
            // dangerouslySetInnerHTML={{ __html: data }}
          >
            <div dangerouslySetInnerHTML={{ __html: data }} />
            {/* {data} */}
          </Card>
        )}
      </div>
    </PageLayout>
  );
};

export default TarotPage;

//  Chỗ này sẽ hiển thị kết quả lá bài sau khi fetch xong

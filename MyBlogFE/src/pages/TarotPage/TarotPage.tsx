import { useState } from "react";
import { Card, Tabs, Typography } from "antd";
import { BulbOutlined, EditOutlined, StarOutlined } from "@ant-design/icons";
import { PageLayout } from "@/components";
import { useSafeTranslation } from "@/hooks";
import GuidedTarot from "./components/GuidedTarot";
import CustomTarot from "./components/CustomTarot";

const { Title, Text } = Typography;

const TarotPage = () => {
  const [activeTab, setActiveTab] = useState("guided");
  const { t } = useSafeTranslation();
  const [data, setData] = useState<string>("");

  return (
    <PageLayout title={t("TarotPage")}>
      <div style={{ maxWidth: 600, margin: "40px auto", padding: "0 20px" }}>
        <Card
          style={{
            borderRadius: 16,
          }}
        >
          <div style={{ textAlign: "center", marginBottom: 24 }}>
            <Title level={2}>
              <StarOutlined style={{ color: "#722ed1", marginRight: 10 }} />

              {t("TarotTitle")}
            </Title>
            <Text type="secondary">{t("TarotCaption")}</Text>
          </div>

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
                  <GuidedTarot
                    isActive={activeTab === "guided"}
                    onReturn={(data) => {
                      setData(data);
                    }}
                  />
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
                  <CustomTarot
                    isActive={activeTab === "custom"}
                    onReturn={(data) => {
                      setData(data);
                    }}
                  />
                ),
              },
            ]}
          />
        </Card>
        {data && (
          <Card
            style={{
              marginTop: 24,
              borderRadius: 12,
              textAlign: "left",
            }}
          >
            <div dangerouslySetInnerHTML={{ __html: data }} />
          </Card>
        )}
      </div>
    </PageLayout>
  );
};

export default TarotPage;

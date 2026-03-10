import { Tabs, Typography } from "antd";
import Card from "antd/es/card/Card";
import { StarOutlined, BulbOutlined, EditOutlined } from "@ant-design/icons";
import { useState } from "react";
import GuidedTarot from "./components/GuidedTarot";
import CustomTarot from "./components/CustomTarot";
import useSafeTranslation from "@/hooks/useSafeTranslation";
import i18n from "@/utils/i18n.util";

const { Title, Text } = Typography;

export default function Popup() {
  const [activeTab, setActiveTab] = useState("guided");
  const { t } = useSafeTranslation();
  const [data, setData] = useState<string>("");

  console.log(i18n.language);

  return (
    <div style={{ width: 450, padding: "16px", background: "#f5f5f5" }}>
      <Card
        style={{ borderRadius: 12, boxShadow: "0 4px 12px rgba(0,0,0,0.1)" }}
      >
        <div style={{ textAlign: "center", marginBottom: 20 }}>
          <Title level={3}>
            <StarOutlined style={{ color: "#722ed1", marginRight: 8 }} />
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
      </Card>
    </div>
  );
}

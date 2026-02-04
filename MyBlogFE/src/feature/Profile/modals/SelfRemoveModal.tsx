import { accountApi } from "@/api";
import { useApiMutation, useAuth, useSafeTranslation } from "@/hooks";
import { App, Button, Modal, Space } from "antd";
import Title from "antd/es/typography/Title";
import { useNavigate } from "react-router-dom";
import dayjs from "dayjs";

type SelfRemoveModalProps = {
  onClose: () => void;
};

const dateFormat = () => {
  var lang = localStorage.getItem("i18nextLng");
  switch (lang) {
    case "vi":
      return "DD/MM/YYYY";
    case "ja":
      return "YYYY年MM月DD日";
    default:
      return "MM/DD/YYYY";
  }
};

const SelfRemoveModal = ({ onClose }: SelfRemoveModalProps) => {
  const { t } = useSafeTranslation();
  const { message } = App.useApp();
  const { logout } = useAuth();
  const navigation = useNavigate();

  const { mutate, isLoading } = useApiMutation({
    mutationKey: ["selfRemove"],
    mutationFn: async () => accountApi.selfRemove(),
    onSuccess: (data) => {
      message.info(
        t("SelfRemoveSuccess") + dayjs(data).format(dateFormat()),
        5,
      );

      logout();
      navigation("/login", { replace: true });
    },
  });

  return (
    <Modal
      title={t("SelfRemove")}
      open={true}
      onCancel={onClose}
      footer={[
        <Button
          onClick={() => onClose()}
          // disabled={isLoading}
        >
          {t("Cancel")}
        </Button>,
        <Button
          type="primary"
          danger
          onClick={() => mutate({})}
          loading={isLoading}
          disabled={isLoading}
        >
          {t("SelfRemove")}
        </Button>,
      ]}
    >
      <Title level={5}>{t("SelfRemoveContent")} </Title>
      <Space vertical size="middle" style={{ width: "100%" }}>
        {t("SelfRemoveContent2")}
        {/* 
        {t("SelfRemoveContent3")}

        <Form>
          <Form.Item
            label={t("Password")}
            name="password"
            rules={[{ required: true, message: t("PasswordEmpty") }]}
          >
            <Input.Password
              placeholder={t("Password")}
              prefix={<LockOutlined />}
            />
          </Form.Item>
        </Form> */}
      </Space>
    </Modal>
  );
};

export default SelfRemoveModal;

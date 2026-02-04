import { useApiMutation, useSafeTranslation } from "@/hooks";
import { App, Button, Form, Input, Modal } from "antd";
import { useForm } from "antd/es/form/Form";
import { LockOutlined } from "@ant-design/icons";
import type { UpdatePasswordRequest } from "@/types/account.type";
import { accountApi } from "@/api";
import type { NamePath } from "antd/es/form/interface";

type ChangePasswordModalProps = {
  onClose: () => void;
};

type ChangePasswordError = Partial<Record<keyof UpdatePasswordRequest, string>>;

const ChangePasswordModal = ({ onClose }: ChangePasswordModalProps) => {
  const { t, tUnsafe } = useSafeTranslation();
  const [form] = useForm();
  const { message } = App.useApp();

  const { mutate, isLoading } = useApiMutation({
    mutationKey: ["changePassword"],
    mutationFn: accountApi.changePassword,
    onSuccess: () => {
      message.success(t("PasswordChanged"));
      onClose();
    },
    onError: (errorData) => {
      // handle error
      handleError(errorData.data as ChangePasswordError);
    },
  });

  const handleSubmit = (values: UpdatePasswordRequest) => {
    mutate(values);
  };

  const handleError = (errorData: ChangePasswordError) => {
    const fields = Object.keys(errorData).map((key) => ({
      name: [key] as NamePath<UpdatePasswordRequest>,
      errors: [tUnsafe(errorData[key as keyof UpdatePasswordRequest]!)],
    }));

    form.setFields(fields);
  };

  return (
    <Modal
      onCancel={onClose}
      open={true}
      title={t("ChangePassword")}
      footer={[
        <Button onClick={() => onClose()}>{t("Cancel")}</Button>,
        <Button
          type="primary"
          onClick={() => form.submit()}
          loading={isLoading}
          disabled={isLoading}
        >
          {t("ChangePassword")}
        </Button>,
      ]}
    >
      <Form
        form={form}
        layout="vertical"
        onFinish={handleSubmit}
        disabled={isLoading}
      >
        <Form.Item<UpdatePasswordRequest>
          name="oldPassword"
          rules={[{ required: true, message: t("CurrentPasswordEmpty") }]}
        >
          <Input.Password
            type="password"
            placeholder={t("CurrentPassword")}
            prefix={<LockOutlined />}
          />
        </Form.Item>

        <Form.Item<UpdatePasswordRequest>
          name="newPassword"
          rules={[
            { required: true, message: t("PasswordEmpty") },
            {
              type: "string",
              min: 8,
              max: 50,
              message: t("PasswordLength"),
            },
            {
              pattern: /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*\W).+$/,
              message: t("PasswordInvalid"),
            },
          ]}
        >
          <Input.Password
            placeholder={t("NewPassword")}
            prefix={<LockOutlined />}
          />
        </Form.Item>

        <Form.Item
          name="confirmPassword"
          dependencies={["newPassword"]}
          rules={[
            { required: true, message: t("PasswordConfirmEmpty") },
            ({ getFieldValue }) => ({
              validator(_, value) {
                if (!value || getFieldValue("newPassword") === value) {
                  return Promise.resolve();
                }
                return Promise.reject(new Error(t("PasswordConfirmMatch")));
              },
            }),
          ]}
        >
          <Input.Password
            placeholder={t("ConfirmNewPassword")}
            prefix={<LockOutlined />}
          />
        </Form.Item>
      </Form>
    </Modal>
  );
};

export default ChangePasswordModal;

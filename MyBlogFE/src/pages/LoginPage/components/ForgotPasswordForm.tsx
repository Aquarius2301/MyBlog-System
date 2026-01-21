import { Button, Form, Input, App } from "antd";
import Title from "antd/es/typography/Title";
import { UserOutlined } from "@ant-design/icons";
import { useApiMutation, useSafeTranslation } from "@/hooks";
import { authApi } from "@/api";
import type { ForgotPasswordRequest } from "@/types/auth.type";

export const ForgotPasswordForm = () => {
  const { t, tUnsafe } = useSafeTranslation();
  const [form] = Form.useForm<ForgotPasswordRequest>();
  const { message } = App.useApp();

  const { isLoading, mutate } = useApiMutation<any, ForgotPasswordRequest>({
    mutationKey: ["forgotPassword"],
    mutationFn: authApi.forgotPassword,
    onSuccess: () => message.info(t("ConfirmEmail")),
    onError: (error) => {
      if (error.data) handleFormError(error.data as ForgotPasswordRequest);
      else message.error(tUnsafe(error.message));
    },
  });

  const handleFormError = (errorData: ForgotPasswordRequest) => {
    if (errorData.hasOwnProperty("identifier")) {
      form.setFields([
        {
          name: "identifier",
          errors: [tUnsafe(errorData.identifier)],
        },
      ]);
    }
  };

  const handleSubmit = (values: ForgotPasswordRequest) => {
    mutate({
      identifier: values.identifier,
    });
  };

  return (
    <Form
      onFinish={handleSubmit}
      form={form}
      autoComplete="off"
      disabled={isLoading}
      style={{
        display: "flex",
        justifyContent: "center",
        alignItems: "center",
        height: "100%",
        flexDirection: "column",
      }}
    >
      <Title level={2} style={{ marginTop: 10 }}>
        {t("ForgotPassword")}
      </Title>
      <Form.Item<ForgotPasswordRequest>
        name="identifier"
        rules={[{ required: true, message: t("UsernameEmailEmpty") }]}
        style={{ width: "100%" }}
      >
        <Input
          prefix={<UserOutlined />}
          placeholder={t("UsernameEmail")}
          style={{ width: "100%" }}
        />
      </Form.Item>

      <Form.Item label={null}>
        <Button type="primary" htmlType="submit" loading={isLoading}>
          {t("SendEmail")}
        </Button>
      </Form.Item>
    </Form>
  );
};

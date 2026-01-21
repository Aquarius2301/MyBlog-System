import { Button, Divider, Form, Input, App } from "antd";
import Title from "antd/es/typography/Title";
import { UserOutlined, LockOutlined } from "@ant-design/icons";
import { useNavigate } from "react-router-dom";
import { useApiMutation, useAuth, useSafeTranslation } from "@/hooks";
import { authApi } from "@/api";
import type { ApiResponse } from "@/types/common.type";
import type { NamePath } from "antd/es/form/interface";
import type { AuthRequest } from "@/types/auth.type";

type LoginError = Partial<Record<keyof AuthRequest, string>>;

export const LoginForm = () => {
  const { t, tUnsafe } = useSafeTranslation();
  const navigate = useNavigate();
  const [form] = Form.useForm<AuthRequest>();
  const { message } = App.useApp();
  const { fetchInfo } = useAuth();

  const { isLoading, mutate } = useApiMutation({
    mutationKey: ["login"],
    mutationFn: authApi.login,
    onSuccess: () => {
      message.success(tUnsafe("LoginSuccess"));
      fetchInfo();
      navigate("/home");
    },
    onError: (errorData: ApiResponse<AuthRequest>) => {
      if (errorData.statusCode === 400 && errorData.data)
        handleFormError(errorData.data);
      else if (errorData.statusCode === 401) {
        message.error(tUnsafe(errorData.message));
      }
    },
  });

  const handleFormError = (errorData: LoginError) => {
    const fields = Object.keys(errorData).map((key) => ({
      name: [key] as NamePath<AuthRequest>,
      errors: [tUnsafe(errorData[key as keyof AuthRequest]!)],
    }));

    form.setFields(fields);
  };

  const handleLogin = (values: AuthRequest) => {
    mutate({
      username: values.username || "",
      password: values.password || "",
    });
  };

  return (
    <Form
      form={form}
      onFinish={handleLogin}
      autoComplete="off"
      disabled={isLoading}
      style={{
        display: "flex",
        justifyContent: "center",
        alignItems: "center",
        flexDirection: "column",
      }}
    >
      <Title level={2} style={{ marginTop: 10 }}>
        {t("Welcome")}
      </Title>
      <Divider>{t("LoginToContinue")}</Divider>
      <Form.Item<AuthRequest>
        name="username"
        rules={[{ required: true, message: t("UsernameEmpty") }]}
        style={{ width: "100%" }}
      >
        <Input
          prefix={<UserOutlined />}
          placeholder={t("Username")}
          style={{ width: "100%" }}
        />
      </Form.Item>

      <Form.Item<AuthRequest>
        name="password"
        rules={[{ required: true, message: t("PasswordEmpty") }]}
        style={{ width: "100%" }}
      >
        <Input.Password
          prefix={<LockOutlined />}
          placeholder={t("Password")}
          style={{ width: "100%" }}
        />
      </Form.Item>

      <Button type="primary" htmlType="submit" loading={isLoading}>
        {t("Login")}
      </Button>
    </Form>
  );
};

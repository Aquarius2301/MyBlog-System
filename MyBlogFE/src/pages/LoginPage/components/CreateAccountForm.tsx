import { Button, DatePicker, Flex, Form, Input, message } from "antd";
import Title from "antd/es/typography/Title";
import {
  CalendarOutlined,
  LockOutlined,
  MailOutlined,
  UserOutlined,
} from "@ant-design/icons";
import dayjs from "dayjs";
import customParseFormat from "dayjs/plugin/customParseFormat";
import { getValidBirthDate } from "@/utils";
import { useApiMutation, useSafeTranslation } from "@/hooks";
import { authApi } from "@/api";
import type { NamePath } from "antd/es/form/interface";
import type { RegisterRequest } from "@/types/auth.type";

type RegisterError = Partial<Record<keyof RegisterRequest, string>>;

dayjs.extend(customParseFormat);

export const CreateAccountForm = () => {
  const { t, tUnsafe } = useSafeTranslation();

  const [form] = Form.useForm<RegisterRequest>();

  const { isLoading, mutate } = useApiMutation<any, RegisterRequest>({
    mutationKey: ["register"],
    mutationFn: authApi.register,
    onSuccess: () => message.info(t("ConfirmEmail")),
    onError: (errorData) => handleFormError(errorData.data as RegisterError),
  });

  const handleFormError = (errorData: RegisterError) => {
    const fields = Object.keys(errorData).map((key) => ({
      name: [key] as NamePath<RegisterRequest>,
      errors: [tUnsafe(errorData[key as keyof RegisterRequest]!)],
    }));

    form.setFields(fields);
  };

  const handleSubmit = (values: RegisterRequest) => {
    var requestData = {
      username: values["username"] || "",
      displayName: values["displayName"] || "",
      password: values["password"] || "",
      email: values["email"] || "",
      dateOfBirth: values["dateOfBirth"]
        ? dayjs(values["dateOfBirth"]).format("YYYY-MM-DD")
        : dayjs(getValidBirthDate()).format("YYYY-MM-DD"),
    };

    mutate(requestData);
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

  return (
    <Form
      form={form}
      onFinish={handleSubmit}
      disabled={isLoading}
      autoComplete="off"
      style={{
        display: "flex",
        justifyContent: "center",
        alignItems: "center",
        height: "100%",
        flexDirection: "column",
      }}
    >
      <Title level={2} style={{ marginTop: 10 }}>
        {t("CreateAccount")}
      </Title>
      <Flex style={{ width: "100%" }} gap={16}>
        <Form.Item<RegisterRequest>
          name="username"
          rules={[
            { required: true, message: t("UsernameEmpty") },
            {
              type: "string",
              min: 3,
              max: 20,
              message: t("UsernameLength"),
            },
            {
              pattern: /^[a-zA-Z0-9_]+$/,
              message: t("UsernameInvalid"),
            },
          ]}
          style={{ width: "100%" }}
        >
          <Input
            prefix={<UserOutlined />}
            placeholder={t("Username")}
            style={{ width: "100%" }}
          />
        </Form.Item>

        <Form.Item<RegisterRequest>
          name="displayName"
          rules={[
            { required: true, message: t("DisplayNameEmpty") },
            {
              type: "string",
              min: 3,
              max: 50,
              message: t("DisplayNameLength"),
            },
          ]}
          style={{ width: "100%" }}
        >
          <Input
            prefix={<UserOutlined />}
            placeholder={t("DisplayName")}
            style={{ width: "100%" }}
          />
        </Form.Item>
      </Flex>

      <Form.Item<RegisterRequest>
        name="password"
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
        style={{ width: "100%" }}
      >
        <Input.Password
          prefix={<LockOutlined />}
          placeholder={t("Password")}
          style={{ width: "100%" }}
        />
      </Form.Item>

      <Form.Item
        name="passwordConfirm"
        dependencies={["password"]}
        rules={[
          { required: true, message: t("PasswordConfirmEmpty") },
          ({ getFieldValue }) => ({
            validator(_, value) {
              if (!value || getFieldValue("password") === value) {
                return Promise.resolve();
              }
              return Promise.reject(new Error(t("PasswordConfirmMatch")));
            },
          }),
        ]}
        style={{ width: "100%" }}
      >
        <Input.Password
          prefix={<LockOutlined />}
          placeholder={t("PasswordConfirm")}
          style={{ width: "100%" }}
        />
      </Form.Item>

      <Form.Item<RegisterRequest>
        name="email"
        rules={[
          { required: true, message: t("EmailEmpty") },
          { type: "email", message: t("EmailInvalid") },
        ]}
        tooltip={"EmailTooltip"}
        style={{ width: "100%" }}
      >
        <Input
          prefix={<MailOutlined />}
          placeholder={t("Email")}
          style={{ width: "100%" }}
        />
      </Form.Item>

      <Form.Item<RegisterRequest>
        name="dateOfBirth"
        rules={[
          {
            validator: (_, value) => {
              if (!value) {
                return Promise.reject(t("DateOfBirthEmpty"));
              }
              // value is a dayjs object from DatePicker
              if (dayjs().diff(value, "year") < 13) {
                return Promise.reject(t("DateOfBirthValid"));
              }
              return Promise.resolve();
            },
          },
        ]}
        style={{ width: "100%" }}
      >
        <DatePicker
          placeholder={t("DateOfBirth")}
          style={{ width: "100%" }}
          maxDate={dayjs(getValidBirthDate(), "YYYY-MM-DD")}
          defaultValue={dayjs(getValidBirthDate(), "YYYY-MM-DD")}
          suffixIcon={null}
          prefix={<CalendarOutlined />}
          format={dateFormat()}
        />
      </Form.Item>

      <Form.Item label={null}>
        <Button type="primary" loading={isLoading} htmlType="submit">
          {t("CreateAccount")}
        </Button>
      </Form.Item>
    </Form>
  );
};

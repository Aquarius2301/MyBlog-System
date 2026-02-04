import { useApiMutation, useAuth, useSafeTranslation } from "@/hooks";
import { getValidBirthDate } from "@/utils";
import { App, Button, DatePicker, Form, Input, Modal, Select } from "antd";
import { useForm } from "antd/es/form/Form";
import { Option } from "antd/es/mentions";
import dayjs from "dayjs";
import { CalendarOutlined } from "@ant-design/icons";
import { useEffect } from "react";
import { accountApi } from "@/api";
import type { NamePath } from "antd/es/form/interface";
import type { UpdateAccountRequest } from "@/types/account.type";

type EditProfileModalProps = {
  onClose: () => void;
};

type UpdateAccountError = Partial<Record<keyof UpdateAccountRequest, string>>;

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

const EditProfileModal = ({ onClose }: EditProfileModalProps) => {
  const { t, tUnsafe } = useSafeTranslation();
  const [form] = useForm();
  const { account, fetchInfo } = useAuth();
  const { message } = App.useApp();

  const { isLoading, mutate } = useApiMutation({
    mutationKey: ["updateProfile"],
    mutationFn: accountApi.updateProfile,
    onSuccess: () => {
      message.success(t("ProfileUpdated"));
      fetchInfo();
      onClose();
    },
    onError: (errorData) => {
      handleFormError(errorData.data as UpdateAccountError);
    },
  });

  const handleFormError = (errorData: UpdateAccountError) => {
    const fields = Object.keys(errorData).map((key) => ({
      name: [key] as NamePath<UpdateAccountRequest>,
      errors: [tUnsafe(errorData[key as keyof UpdateAccountRequest]!)],
    }));

    form.setFields(fields);
  };

  useEffect(() => {
    if (!account) return;

    form.setFieldsValue({
      username: account.username,
    });
    form.setFieldsValue({
      displayName: account.displayName,
    });
    form.setFieldsValue({
      email: account.email,
    });
    form.setFieldsValue({
      dateOfBirth: dayjs(account.dateOfBirth, "YYYY-MM-DD"),
    });
    form.setFieldsValue({
      language: account.language,
    });
  }, [account, form]);

  const handleSubmit = (values: UpdateAccountRequest) => {
    var requestData = {
      //   username: values["username"] || "",
      displayName: values["displayName"] || "",
      //   password: values["password"] || "",
      //   email: values["email"] || "",
      dateOfBirth: values["dateOfBirth"]
        ? dayjs(values["dateOfBirth"]).format("YYYY-MM-DD")
        : dayjs(getValidBirthDate()).format("YYYY-MM-DD"),
      language: values["language"] || "en",
    };

    mutate(requestData);
  };

  return (
    <Modal
      open={true}
      onCancel={onClose}
      title={t("UpdateProfile")}
      footer={[
        <Button
          //   type="primary"
          onClick={() => onClose()}
          disabled={isLoading}
        >
          {t("Cancel")}
        </Button>,
        <Button
          type="primary"
          onClick={() => form.submit()}
          loading={isLoading}
          disabled={isLoading}
        >
          {t("UpdateProfile")}
        </Button>,
      ]}
    >
      <Form
        onFinish={handleSubmit}
        form={form}
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
        {/* <Form.Item<UpdateAccountRequest>
          name="username"
          style={{ width: "100%" }}
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
        >
          <Input placeholder={t("Username")} prefix="@" />
        </Form.Item> */}
        <Form.Item<UpdateAccountRequest>
          name="displayName"
          style={{ width: "100%" }}
          rules={[
            { required: true, message: t("DisplayNameEmpty") },
            {
              type: "string",
              min: 3,
              max: 50,
              message: t("DisplayNameLength"),
            },
          ]}
        >
          <Input placeholder={t("DisplayName")} />
        </Form.Item>
        {/* <Form.Item
          name="email"
          style={{ width: "100%" }}
          rules={[
            { required: true, message: t("EmailEmpty") },
            { type: "email", message: t("EmailInvalid") },
          ]}
        >
          <Input placeholder={t("Email")} />
        </Form.Item> */}
        <Form.Item<UpdateAccountRequest>
          name="dateOfBirth"
          style={{ width: "100%" }}
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
        <Form.Item<UpdateAccountRequest>
          name="language"
          style={{ width: "100%" }}
        >
          <Select placeholder={t("Language")}>
            <Option value="en">{t("English")}</Option>
            <Option value="ja">{t("Japanese")}</Option>
            <Option value="vi">{t("Vietnamese")}</Option>
          </Select>
        </Form.Item>
      </Form>
    </Modal>
  );
};

export default EditProfileModal;

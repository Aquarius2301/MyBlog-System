// import { BackgroundColor } from "@/components/style.type";
// import {
//   Button,
//   Card,
//   Flex,
//   Form,
//   Input,
//   Result,
//   Spin,
//   message as mes,
// } from "antd";
// import { useEffect, useRef, useState } from "react";
// import { LockOutlined } from "@ant-design/icons";
// import { authApi } from "@/api";
// import { useNavigate } from "react-router-dom";
// import { useSafeTranslation, useTheme } from "@/hooks/";

// type Props = {
//   type: string;
//   token: string;
// };

// type FieldType = {
//   password: string;
//   passwordConfirm: string;
// };

// export const ResetPasswordConfirm = ({ type, token }: Props) => {
//   const [message, setMessage] = useState("");
//   const [loading, setLoading] = useState(true);
//   const [finishForm, setFinishForm] = useState(true);
//   const { themeMode } = useTheme();
//   const hasFetched = useRef(false);

//   const navigate = useNavigate();
//   const { t } = useSafeTranslation();

//   useEffect(() => {
//     const fetchConfirm = async () => {
//       if (hasFetched.current) return; // Avoid run twice in StrictMode
//       hasFetched.current = true;

//       setLoading(true);

//       if (type && token) {
//         var res = await authApi.confirm(type, token);
//         if (res.statusCode === 200) {
//           setMessage(res.message);
//         } else {
//           navigate("/notFound?message=TokenMissing");
//         }
//       }
//       setLoading(false);
//     };

//     fetchConfirm();
//   }, []);

//   const handleResetPassword = async (values: FieldType) => {
//     setLoading(true);

//     var res = await authApi.resetPassword({
//       confirmCode: token,
//       newPassword: values.password,
//     });

//     if (res.statusCode === 200) {
//       setMessage(res.message);
//       setFinishForm(false);
//     } else {
//       mes.error(res.message);
//     }

//     setLoading(false);
//   };

//   return loading ? (
//     <Spin size="large" fullscreen />
//   ) : (
//     <Flex
//       justify="center"
//       align="center"
//       style={{
//         backgroundColor: BackgroundColor[themeMode],
//         height: "100vh",
//         width: "100vw",
//       }}
//     >
//       <Card>
//         <Result status={"success"} title={message} />
//         {finishForm && (
//           <Form
//             onFinish={handleResetPassword}
//             autoComplete="off"
//             style={{
//               display: "flex",
//               flexDirection: "column",
//               gap: 16,
//               width: "100%",
//             }}
//           >
//             <Form.Item<FieldType>
//               name="password"
//               rules={[
//                 { required: true, message: t("PasswordEmpty") },
//                 {
//                   type: "string",
//                   min: 8,
//                   max: 50,
//                   message: t("PasswordLength"),
//                 },
//                 {
//                   pattern: /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*\W).+$/,
//                   message: t("PasswordInvalid"),
//                 },
//               ]}
//               style={{ width: "100%" }}
//             >
//               <Input.Password
//                 prefix={<LockOutlined />}
//                 placeholder={t("NewPassword")}
//                 style={{ width: "100%" }}
//               />
//             </Form.Item>

//             <Form.Item<FieldType>
//               name="passwordConfirm"
//               dependencies={["password"]}
//               rules={[
//                 { required: true, message: t("PasswordConfirmEmpty") },
//                 ({ getFieldValue }) => ({
//                   validator(_, value) {
//                     if (!value || getFieldValue("password") === value) {
//                       return Promise.resolve();
//                     }
//                     return Promise.reject(new Error(t("PasswordConfirmMatch")));
//                   },
//                 }),
//               ]}
//               style={{ width: "100%" }}
//             >
//               <Input.Password
//                 prefix={<LockOutlined />}
//                 placeholder={t("PasswordConfirm")}
//                 style={{ width: "100%" }}
//               />
//             </Form.Item>
//             <Form.Item>
//               <Button type="primary" htmlType="submit">
//                 {t("ChangePassword")}
//               </Button>
//             </Form.Item>
//           </Form>
//         )}
//       </Card>
//     </Flex>
//   );
// };

import { BackgroundColor } from "@/components/style.type";
import { Button, Card, Flex, Form, Input, Result, Spin } from "antd";
import { LockOutlined } from "@ant-design/icons";
import { authApi } from "@/api";
import {
  useSafeTranslation,
  useTheme,
  useApiQuery,
  useApiMutation,
} from "@/hooks";
import { useState } from "react";

type Props = {
  type: string;
  token: string;
};

type FieldType = {
  password: string;
  passwordConfirm: string;
};

export const ResetPasswordConfirm = ({ type, token }: Props) => {
  const { themeMode } = useTheme();
  const { t } = useSafeTranslation();

  const [finishForm, setFinishForm] = useState(true);
  const [message, setMessage] = useState(t("ConfirmSuccess2"));

  /**
   * 1. Confirm token (Query)
   */
  const { error: confirmError, isLoading: isConfirmLoading } = useApiQuery({
    queryKey: ["confirm", type, token],
    queryFn: () => authApi.confirm(type, token),
    enabled: !!type && !!token,
  });

  /**
   * 2. Reset password (Mutation)
   */
  const resetPasswordMutation = useApiMutation({
    mutationFn: (values: FieldType) =>
      authApi.resetPassword({
        confirmCode: token,
        newPassword: values.password,
      }),
    onSuccess: () => {
      setMessage(t("PasswordChanged&Login"));
      setFinishForm(false);
    },
  });

  const handleResetPassword = (values: FieldType) => {
    resetPasswordMutation.mutate(values);
  };

  /**
   * Loading confirm
   */
  if (isConfirmLoading) {
    return <Spin size="large" fullscreen />;
  }

  /**
   * Token invalid
   */
  if (confirmError) {
    return (
      <Card style={{ width: "100vw", height: "100vh" }}>
        <Result status="404" title={t("TokenMissing")} />
      </Card>
    );
  }

  /**
   * Success confirm
   */
  return (
    <Flex
      justify="center"
      align="center"
      style={{
        backgroundColor: BackgroundColor[themeMode],
        height: "100vh",
        width: "100vw",
      }}
    >
      <Card>
        <Result status="success" title={message} />

        {finishForm && (
          <Form
            onFinish={handleResetPassword}
            autoComplete="off"
            style={{
              display: "flex",
              flexDirection: "column",
              gap: 16,
              width: "100%",
            }}
          >
            <Form.Item<FieldType>
              name="password"
              rules={[
                { required: true, message: t("PasswordEmpty") },
                { min: 8, max: 50, message: t("PasswordLength") },
                {
                  pattern: /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*\W).+$/,
                  message: t("PasswordInvalid"),
                },
              ]}
            >
              <Input.Password
                prefix={<LockOutlined />}
                placeholder={t("NewPassword")}
              />
            </Form.Item>

            <Form.Item<FieldType>
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
            >
              <Input.Password
                prefix={<LockOutlined />}
                placeholder={t("PasswordConfirm")}
              />
            </Form.Item>

            <Button
              type="primary"
              htmlType="submit"
              loading={resetPasswordMutation.isLoading}
            >
              {t("ChangePassword")}
            </Button>
          </Form>
        )}
      </Card>
    </Flex>
  );
};

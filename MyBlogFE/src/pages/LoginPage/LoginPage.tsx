import {
  Button,
  Card,
  Divider,
  Flex,
  Switch,
  Tabs,
  type TabsProps,
} from "antd";
import {
  Background,
  CreateAccountForm,
  ForgotPasswordForm,
  LoginForm,
} from "./components";
import "./styles.css";
import { useSafeTranslation, useTheme } from "@/hooks";
import { BackgroundColor } from "@/components/style.type";
import { Header } from "antd/es/layout/layout";
import { MoonOutlined, SunOutlined } from "@ant-design/icons";

const LoginPage = () => {
  const { t, i18n } = useSafeTranslation();
  const { themeMode, toggleTheme } = useTheme();

  const changeLanguage = (lang: string) => {
    i18n.changeLanguage(lang);
    localStorage.setItem("i18nextLng", lang);
  };

  const items: TabsProps["items"] = [
    {
      key: "login",
      label: t("Login"),
      children: <LoginForm />,
    },
    {
      key: "createAccount",
      label: t("CreateAccount"),
      children: <CreateAccountForm />,
    },
    {
      key: "forgotPassword",
      label: t("ForgotPassword"),
      children: <ForgotPasswordForm />,
    },
  ];

  return (
    <>
      <Header
        style={{
          backgroundColor: BackgroundColor[themeMode],
          padding: "10px 10px",
        }}
      >
        {/* <Typography.Title level={4} style={{ margin: 0 }}>
          {t("LoginPage")}
        </Typography.Title> */}
        <Switch
          checkedChildren={<SunOutlined />}
          unCheckedChildren={<MoonOutlined />}
          checked={themeMode === "light"}
          onChange={toggleTheme}
        />
      </Header>
      <Flex
        justify="center"
        align="center"
        style={{
          backgroundColor: BackgroundColor[themeMode],
          height: "100vh",
          width: "100vw",
        }}
      >
        <Card // .ant-card
          style={{
            width: "60%",
            height: "70%",
            // backgroundColor: "transparent",
          }}
          styles={{ body: { width: "100%", height: "100%" } }}
        >
          <Flex style={{ height: "100%" }}>
            {/* Left side background image */}
            <Background />

            {/* Right side login form */}
            <Card
              style={{
                marginLeft: "10px",
                width: "100%",
                border: "none",
              }}
              styles={{ body: { height: "100%" } }}
            >
              <Flex vertical style={{ height: "100%" }}>
                {/*Tabs for Login, Create Account, Forgot Password */}
                <Tabs
                  defaultActiveKey="1"
                  items={items}
                  style={{ borderBlock: "none", flex: 1 }}
                />

                {/* Language */}
                <Divider>
                  {t("Language")}
                  {": "}
                  <Button
                    type="link"
                    onClick={() => changeLanguage("en")}
                    style={{ padding: 0 }}
                  >
                    English
                  </Button>
                  {" | "}
                  <Button
                    type="link"
                    onClick={() => changeLanguage("vi")}
                    style={{ padding: 0 }}
                  >
                    Tiếng Việt
                  </Button>
                  {" | "}
                  <Button
                    type="link"
                    onClick={() => changeLanguage("ja")}
                    style={{ padding: 0 }}
                  >
                    日本語
                  </Button>
                </Divider>
              </Flex>
            </Card>
          </Flex>
        </Card>
      </Flex>
    </>
    // </LoginNavBar>
  );
};

export default LoginPage;

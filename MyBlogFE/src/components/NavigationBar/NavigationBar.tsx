import {
  Affix,
  Button,
  Dropdown,
  Flex,
  Switch,
  Typography,
  type MenuProps,
} from "antd";
import { useNavigate } from "react-router-dom";
import {
  LogoutOutlined,
  MessageOutlined,
  MoonOutlined,
  SearchOutlined,
  StarOutlined,
  SunOutlined,
  UserOutlined,
} from "@ant-design/icons";

import { Header } from "antd/es/layout/layout";
import { useAuth, useSafeTranslation, useTheme } from "@/hooks";
import { BackgroundColor } from "../style.type";
import { ImageAvatar } from "../ImageAvatar";

export type NavigationBarProps = {
  title?: string;
};

const NavigationBar = ({ title }: NavigationBarProps) => {
  const navigate = useNavigate();
  const { account, logout } = useAuth();
  const { t } = useSafeTranslation();
  const { themeMode, toggleTheme } = useTheme();

  const userMenu: MenuProps["items"] = [
    { key: "/profile/me", label: t("Profile"), icon: <UserOutlined /> },
    { type: "divider" },
    {
      label: t("Logout"),
      key: "/login",
      onClick: () => {
        logout();
        navigate("/login", { replace: true });
      },
      icon: <LogoutOutlined />,
      danger: true,
    },
  ];

  return (
    <Affix>
      <Header
        style={{
          backgroundColor: BackgroundColor[themeMode],
        }}
      >
        <Flex justify="space-between" align="center">
          <div>
            <div
              onClick={() => navigate("/home")}
              style={{ cursor: "pointer", display: "inline-block" }}
            >
              <ImageAvatar url="/favicon.png" shape="square" />
              <span style={{ margin: 10, fontWeight: "bold" }}>My Blog</span>
            </div>

            <Switch
              onChange={toggleTheme}
              checkedChildren={<SunOutlined />}
              unCheckedChildren={<MoonOutlined />}
              checked={themeMode === "light"}
            />
            <Button
              style={{ marginLeft: 10 }}
              onClick={() => navigate("/tarot")}
              icon={<StarOutlined />}
              type="text"
            />
            <Button
              style={{ marginLeft: 10 }}
              onClick={() => navigate("/chat")}
              icon={<MessageOutlined />}
              type="text"
            />
          </div>

          <div>
            <Typography.Title level={4} style={{ margin: 0 }}>
              {title}
            </Typography.Title>
          </div>
          <div>
            <SearchOutlined
              style={{
                display: "inline-block",
                marginRight: 20,
                cursor: "pointer",
              }}
              onClick={() => navigate("/account/search")}
            />
            <div style={{ display: "inline-block", marginRight: 20 }}>
              <Dropdown
                menu={{ items: userMenu, onClick: (e) => navigate(e.key) }}
                trigger={["click"]}
              >
                <div style={{ cursor: "pointer" }}>
                  <ImageAvatar url={account?.avatarUrl} />
                  <span style={{ marginLeft: 10, fontWeight: "bold" }}>
                    {account?.displayName}
                  </span>
                </div>
              </Dropdown>
            </div>
          </div>
        </Flex>
      </Header>
    </Affix>
  );
};

export default NavigationBar;

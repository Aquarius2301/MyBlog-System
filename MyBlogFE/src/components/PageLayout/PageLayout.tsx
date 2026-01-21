import { Layout } from "antd";
import "./styles/PageLayout.css";
import { NavigationBar } from "../NavigationBar";
import { useEffect } from "react";
import { Content } from "antd/es/layout/layout";

export type PageLayoutProps = {
  children: React.ReactNode;
  title?: string;
  contentCentered?: boolean;
};

const PageLayout = ({
  children,
  title,
  contentCentered = false,
}: PageLayoutProps) => {
  useEffect(() => {
    document.title = "My Blog" + (title ? ` - ${title}` : "");
  }, [title]);

  return (
    <Layout style={{ minHeight: "100vh" }}>
      <NavigationBar title={title} />
      <Layout>
        {/* <SideBar /> */}
        <Content className="main-content">
          <div
            className="content-wrapper"
            style={{
              justifyContent: contentCentered ? "center" : "flex-start",
            }}
          >
            {children}
          </div>
        </Content>
      </Layout>
    </Layout>
  );
};

export default PageLayout;

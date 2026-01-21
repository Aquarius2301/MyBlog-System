import { Card, Flex, Skeleton } from "antd";

const ProfileLoading = () => {
  return (
    <Card style={{ marginBottom: 24 }}>
      <Flex vertical align="center" gap={8}>
        {/* Menu icon */}
        <div style={{ alignSelf: "flex-end", width: 24 }}>
          <Skeleton.Button size="small" shape="circle" active />
        </div>

        {/* Avatar */}
        <Skeleton.Avatar size={128} shape="circle" active />

        {/* Name + username */}
        <div style={{ textAlign: "center", width: "100%" }}>
          <Skeleton.Input
            style={{ width: 160, marginBottom: 8 }}
            size="large"
            active
          />
          {/* <Skeleton.Input style={{ width: 120 }} size="small" active /> */}
        </div>

        {/* Status tag */}
        <Skeleton.Button style={{ width: 80 }} size="small" active />

        {/* Info card */}
        <Card style={{ marginTop: 16, width: "100%" }}>
          <Skeleton active paragraph={{ rows: 3 }} />
        </Card>
      </Flex>
    </Card>
  );
};

export default ProfileLoading;

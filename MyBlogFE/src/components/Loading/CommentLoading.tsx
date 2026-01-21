import { Flex, Skeleton } from "antd";

const CommentLoading = () => {
  return (
    <>
      <Flex
        justify="space-between"
        align="center"
        style={{ marginBottom: "20px" }}
      >
        <div>
          <Skeleton.Avatar size="default" active shape="circle" />
          <Skeleton.Input
            style={{ width: 200, marginLeft: 8 }}
            active
            size="small"
          />
        </div>
        <Skeleton.Input style={{ width: 100 }} active size="small" />
      </Flex>

      {/* Content */}
      <Skeleton active paragraph={{ rows: 3 }} />
      {/* Images */}
      <Skeleton active paragraph={{ rows: 2 }} />
    </>
  );
};

export default CommentLoading;

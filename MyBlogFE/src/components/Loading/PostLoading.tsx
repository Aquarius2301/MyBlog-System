import { Flex, Skeleton } from "antd";
import SkeletonImage from "antd/es/skeleton/Image";

const PostLoading = () => {
  return (
    <>
      <Flex
        justify="flex-start"
        align="center"
        style={{ marginBottom: "20px" }}
      >
        <div>
          <Skeleton.Avatar
            active
            size={50}
            shape="square"
            style={{ marginRight: "8px" }}
          />
        </div>
        <div style={{ width: "50%" }}>
          <Skeleton active paragraph={false} />
          <Skeleton active paragraph={false} />
        </div>
      </Flex>

      <Skeleton active paragraph={{ rows: 2 }} style={{ marginLeft: "auto" }} />

      <SkeletonImage
        style={{ width: "200px", height: "100px", marginTop: 16 }}
      />

      <Skeleton active paragraph={false} style={{ marginTop: 16 }} />
    </>
  );
};

export default PostLoading;

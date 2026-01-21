import { PostLoading } from "@/components";
import { Card } from "antd";

type PostLoadingCardProps = {
  isList?: boolean;
};

const PostLoadingCard = ({ isList = false }: PostLoadingCardProps) => {
  return (
    <>
      <Card style={{ width: "100%", marginBottom: "20px" }}>
        <PostLoading />
      </Card>
      {isList &&
        Array.from({ length: 4 }).map((_, index) => (
          <Card key={index} style={{ width: "100%", marginBottom: "20px" }}>
            <PostLoading />
          </Card>
        ))}
    </>
  );
};

export default PostLoadingCard;

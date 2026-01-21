import { Avatar } from "antd";
import { UserOutlined } from "@ant-design/icons";

export type ImageAvatarProps = {
  url?: string;
  size?: "small" | "default" | "large" | number;
  shape?: "circle" | "square";
};

const ImageAvatar = (props: ImageAvatarProps) => {
  const { url, size = "default", shape = "circle" } = props;

  return <Avatar size={size} shape={shape} src={url} icon={<UserOutlined />} />;
};

export default ImageAvatar;

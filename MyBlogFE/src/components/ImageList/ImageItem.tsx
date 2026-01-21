import { Image } from "antd";

export type ImageItemProps = {
  src: string;
  width?: number;
  height?: number;
  isBlurred?: boolean;
  isHidden?: boolean;
};

const ImageItem = (props: ImageItemProps) => {
  const { src, width, height, isBlurred = false } = props;
  return (
    <Image
      src={src}
      width={width}
      height={height}
      style={{
        objectFit: "cover", //crop the picture
        objectPosition: "center", //focus on the center of the picture
        filter: isBlurred ? "brightness(50%)" : "none",
        display: props.isHidden ? "none" : "block",
      }}
    />
  );
};

export default ImageItem;

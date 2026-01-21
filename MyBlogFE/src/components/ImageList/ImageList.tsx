import { Flex, Image } from "antd";
import { useEffect, useRef, useState } from "react";
import ImageItem from "./ImageItem";

export type ImageListProps = {
  pictureUrls: string[];
};

const ImageList = ({ pictureUrls }: ImageListProps) => {
  const parentRef = useRef<HTMLDivElement>(null);
  const [width, setWidth] = useState<number>(0);
  const [isReady, setIsReady] = useState(false);

  const getImageWidth = (index: number, length: number) => {
    const gap = 10;
    switch (length) {
      case 1:
        return width;
      case 2:
        return (width - gap) / 2;
      case 3:
        return index < 2 ? (width - gap) / 2 : width;
      case 4:
        return (width - gap) / 2;
      default:
        return index < 3 ? (width - gap * 2) / 3 : (width - gap) / 2;
    }
  };

  useEffect(() => {
    if (!parentRef.current) return;

    const update = () => {
      if (!parentRef.current) return;
      const newWidth = parentRef.current.getBoundingClientRect().width;
      if (newWidth > 0) {
        setWidth(newWidth);
        setIsReady(true);
      }
    };

    update();

    setTimeout(update, 1000); // in case of late rendering

    const observer = new ResizeObserver(update);
    observer.observe(parentRef.current);

    return () => observer.disconnect();
  }, []);

  return (
    <Flex
      justify="space-between"
      gap={10}
      wrap
      ref={parentRef}
      style={{
        width: "100%",
        opacity: isReady ? 1 : 0,
      }}
    >
      {pictureUrls.length > 0 && (
        <Image.PreviewGroup>
          {pictureUrls.slice(0, 5).map((src, i) =>
            i === 4 && pictureUrls.length > 5 ? (
              <div
                key={i}
                style={{
                  position: "relative",
                  height: 300,
                  width: getImageWidth(i, pictureUrls.length),
                  overflow: "hidden",
                }}
              >
                <ImageItem
                  src={src}
                  width={getImageWidth(i, pictureUrls.length)}
                  height={300}
                  isBlurred={true}
                  isHidden={false}
                />
                <div
                  style={{
                    position: "absolute",
                    top: "50%",
                    left: "50%",
                    transform: "translate(-50%, -50%)",
                    color: "white",
                    fontSize: 24,
                    fontWeight: "bold",
                  }}
                >
                  +{pictureUrls.length - 5}
                </div>
                {pictureUrls.slice(5).map((srcHidden, j) => (
                  <ImageItem
                    key={j}
                    src={srcHidden}
                    isBlurred={true}
                    isHidden={false}
                  />
                ))}
              </div>
            ) : (
              <ImageItem
                key={i}
                src={src}
                width={getImageWidth(i, pictureUrls.length)}
                height={pictureUrls.length > 2 ? 300 : 400}
              />
            )
          )}
        </Image.PreviewGroup>
      )}
    </Flex>
  );
};

export default ImageList;

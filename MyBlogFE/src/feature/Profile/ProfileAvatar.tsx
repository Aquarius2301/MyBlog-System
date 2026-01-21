import { ImageAvatar } from "@/components";
import { CameraOutlined } from "@ant-design/icons";
import { useState } from "react";
import { ChangeAvatarModal } from "./modals/ChangeAvatarModal";
import { useTheme } from "@/hooks";
import { BackgroundColor } from "@/components/style.type";

type ProfileAvatarProps = {
  url?: string;
  size: number;
  editable?: boolean;
};

export const ProfileAvatar = ({ url, size, editable }: ProfileAvatarProps) => {
  const [modalVisible, setModalVisible] = useState(false);
  const { themeMode } = useTheme();

  return (
    <div
      style={{
        position: "relative",
        width: size,
        height: size,
      }}
    >
      <ImageAvatar url={url} size={size} />
      {editable && (
        <>
          <CameraOutlined
            onClick={() => setModalVisible(true)}
            style={{
              position: "absolute",
              bottom: 0,
              right: 0,
              background: BackgroundColor[themeMode],
              borderRadius: "50%",
              padding: 6,
              fontSize: 16,
              cursor: "pointer",
            }}
          />
          <ChangeAvatarModal
            oldAvatarUrl={url}
            visible={modalVisible}
            onClose={() => setModalVisible(false)}
          />
        </>
      )}
    </div>
  );
};

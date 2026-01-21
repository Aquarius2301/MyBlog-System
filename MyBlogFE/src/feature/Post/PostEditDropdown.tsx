import { Dropdown, type MenuProps } from "antd";
import {
  DeleteOutlined,
  EllipsisOutlined,
  FormOutlined,
} from "@ant-design/icons";
import { useSafeTranslation } from "@/hooks";
import type { GetPostDetailData } from "@/types/post.type";

type PostEditDropdownProps = {
  post: GetPostDetailData;
  onUpdate: () => void;
  onDelete: () => void;
  // setActiveModal: (modal: "update" | "delete" | null) => void;
  // activeModal: "update" | "delete" | null;
};

export const PostEditDropdown = ({
  onUpdate,
  onDelete,
}: PostEditDropdownProps) => {
  const { t } = useSafeTranslation();

  const handleMenuClick = (key: string) => {
    if (key.includes("update")) {
      onUpdate();
    } else if (key.includes("delete")) {
      onDelete();
    }
  };

  const postMenu: MenuProps["items"] = [
    {
      key: `update`,
      label: t("UpdatePost"),
      icon: <FormOutlined />,
    },
    {
      key: `delete`,
      label: t("DeletePost"),
      icon: <DeleteOutlined />,
      danger: true,
    },
  ];

  return (
    <Dropdown
      menu={{ items: postMenu, onClick: (e) => handleMenuClick(e.key) }}
      trigger={["click"]}
    >
      <EllipsisOutlined />
    </Dropdown>
  );
};

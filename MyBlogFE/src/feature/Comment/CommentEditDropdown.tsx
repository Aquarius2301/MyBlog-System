import { Dropdown, type MenuProps } from "antd";
import {
  DeleteOutlined,
  EllipsisOutlined,
  FormOutlined,
} from "@ant-design/icons";
import { useSafeTranslation } from "@/hooks";

type CommentEditDropdownProps = {
  onUpdate: () => void;
  onDelete: () => void;
  // setActiveModal: (modal: "update" | "delete" | null) => void;
  // activeModal: "update" | "delete" | null;
};

export const CommentEditDropdown = ({
  onUpdate,
  onDelete,
}: CommentEditDropdownProps) => {
  const { t } = useSafeTranslation();

  const handleMenuClick = (key: string) => {
    if (key.includes("update")) {
      onUpdate();
    } else if (key.includes("delete")) {
      onDelete();
    }
  };

  const commentMenu: MenuProps["items"] = [
    {
      key: `update`,
      label: t("UpdateComment"),
      icon: <FormOutlined />,
    },
    {
      key: `delete`,
      label: t("DeleteComment"),
      icon: <DeleteOutlined />,
      danger: true,
    },
  ];

  return (
    <Dropdown
      menu={{ items: commentMenu, onClick: (e) => handleMenuClick(e.key) }}
      trigger={["click"]}
    >
      <EllipsisOutlined />
    </Dropdown>
  );
};

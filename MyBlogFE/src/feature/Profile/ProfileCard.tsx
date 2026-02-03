import { formatDate, formatDateTime } from "@/utils";
import { Card, Dropdown, Flex, Tag } from "antd";
import { Text } from "@/components";
import { ProfileAvatar } from "./ProfileAvatar";
import { useSafeTranslation } from "@/hooks";
import type { AccountData } from "@/types/account.type";
import {
  DeleteFilled,
  EditOutlined,
  EllipsisOutlined,
  LockOutlined,
} from "@ant-design/icons";
import EditProfileModal from "./modals/EditProfileModal";
import { useState } from "react";
import SelfRemoveModal from "./modals/SelfRemoveModal";
import ChangePasswordModal from "./modals/ChangePasswordModal";

export type ProfileCardProps = {
  account: AccountData;
};

type ModalType = "updateProfile" | "changePassword" | "selfRemove";
const ProfileCard = ({ account }: ProfileCardProps) => {
  const [openModal, setModalOpen] = useState<ModalType | null>(null);

  const { t } = useSafeTranslation();

  const profileMenu = [
    {
      key: "updateProfile" as ModalType,
      label: t("UpdateProfile"),
      icon: <EditOutlined />,
    },
    {
      key: "changePassword" as ModalType,
      label: t("ChangePassword"),
      icon: <LockOutlined />,
    },
    {
      key: "selfRemove" as ModalType,
      label: t("SelfRemove"),
      icon: <DeleteFilled />,
      danger: true,
    },
  ];

  return (
    <Card style={{ marginBottom: 24 }}>
      <Flex vertical align="center" gap={8}>
        <ProfileAvatar
          url={account.avatarUrl}
          size={128}
          editable={account.isOwner}
        />

        <div style={{ textAlign: "center" }}>
          <Text as={"p"} bold fontSize="xxlarge">
            {account.displayName}
          </Text>
          <Text
            as={"p"}
            style={{ color: "rgba(162, 162, 162, 1)", marginTop: 0 }}
          >
            @{account.username}
          </Text>
        </div>
        <Tag variant="outlined" color={"blue"}>
          {account.status}
        </Tag>
        <Card style={{ marginTop: 16, width: "100%" }}>
          {account.isOwner && (
            <div style={{ textAlign: "right" }}>
              <Dropdown
                menu={{
                  items: profileMenu,
                  onClick: ({ key }) => setModalOpen(key as ModalType),
                }}
                trigger={["click"]}
              >
                <EllipsisOutlined />
              </Dropdown>
            </div>
          )}
          <Text as={"p"} style={{ marginBottom: 16 }}>
            <Text style={{ fontWeight: "bold" }}>Email:</Text> {account.email}
          </Text>
          <Text as={"p"} style={{ marginBottom: 16 }}>
            <Text style={{ fontWeight: "bold" }}>Date of Birth: </Text>
            {formatDate(account.dateOfBirth)}
          </Text>
          <Text as={"p"} style={{ marginBottom: 0 }}>
            <Text style={{ fontWeight: "bold" }}>Joined at:</Text>{" "}
            {formatDateTime(account.createdAt)}
          </Text>
        </Card>
      </Flex>

      {openModal == "updateProfile" && (
        <EditProfileModal onClose={() => setModalOpen(null)} />
      )}

      {openModal == "selfRemove" && (
        <SelfRemoveModal onClose={() => setModalOpen(null)} />
      )}

      {openModal == "changePassword" && (
        <ChangePasswordModal onClose={() => setModalOpen(null)} />
      )}
    </Card>
  );
};

export default ProfileCard;

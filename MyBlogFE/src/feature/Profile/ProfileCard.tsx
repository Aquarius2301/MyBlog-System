import { formatDate, formatDateTime } from "@/utils";
import { Card, Dropdown, Flex, Tag } from "antd";
import { EllipsisOutlined } from "@ant-design/icons";
import { Text } from "@/components";
import { ProfileAvatar } from "./ProfileAvatar";
import { useSafeTranslation } from "@/hooks";
import type { AccountData } from "@/types/account.type";

export type ProfileCardProps = {
  account: AccountData;
};

const ProfileCard = ({ account }: ProfileCardProps) => {
  // const [isModalOpen, setModalOpen] = useState(false);

  const { t } = useSafeTranslation();

  // const handleNavigate = (key: string) => {
  //   if (key === "createPost") {
  //     setModalOpen(true);
  //   }
  // };

  const profileMenu = [
    {
      key: "createPost",
      label: t("CreatePost"),
    },
    {
      key: "updateProfile",
      label: t("UpdateProfile"),
    },
  ];

  return (
    <Card style={{ marginBottom: 24 }}>
      <Flex vertical align="center" gap={8}>
        {account.isOwner && (
          <div style={{ alignSelf: "flex-end" }}>
            <Dropdown
              menu={{
                items: profileMenu,
                // onClick: (e) => handleNavigate(e.key),
              }}
              trigger={["click"]}
            >
              <EllipsisOutlined />
            </Dropdown>
          </div>
        )}
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
    </Card>
  );
};

export default ProfileCard;

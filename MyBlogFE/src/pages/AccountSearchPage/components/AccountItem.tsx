import type { AccountNameData } from "@/types/account.type";
import { formatDate } from "@/utils";
import { Card } from "antd";
import { useNavigate } from "react-router-dom";

type AccountItemProps = {
  account: AccountNameData;
};

export const AccountItem = ({ account }: AccountItemProps) => {
  const navigate = useNavigate();

  return (
    <Card key={account.id} style={{ marginTop: 8 }}>
      <p
        style={{ margin: 0, marginBottom: 4, cursor: "pointer" }}
        onClick={() => {
          navigate(`/profile?u=${account.username}`);
        }}
      >
        <strong>{account.displayName}</strong> (@{account.username})
      </p>
      <p style={{ margin: 0, color: "#666", fontSize: "14px" }}>
        Joined at: {formatDate(account.createdAt)}
      </p>
    </Card>
  );
};

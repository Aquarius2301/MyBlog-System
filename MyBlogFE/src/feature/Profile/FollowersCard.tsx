import { followApi } from "@/api";
import { ImageAvatar, Text } from "@/components";
import { useApiInfiniteQuery, useSafeTranslation } from "@/hooks";
import type { AccountData } from "@/types/account.type";
import { useQueryClient } from "@tanstack/react-query";
import { Card } from "antd";
import InfiniteScroll from "react-infinite-scroll-component";

export type FollowersCardProps = {
  account: AccountData;
};

const FollowersCard = ({ account }: FollowersCardProps) => {
  const queryClient = useQueryClient();
  const { t } = useSafeTranslation();
  const { data, hasNextPage, fetchNextPage } = useApiInfiniteQuery({
    queryKey: ["followers", account.username],
    queryFn: (pageParam) => followApi.getFollowers(account.id, pageParam),
    initialPageParam: null,
  });

  return (
    <Card title={t("Followers")} style={{ marginBottom: 20 }}>
      <InfiniteScroll
        dataLength={data?.length || 0}
        next={fetchNextPage}
        hasMore={hasNextPage || data == null}
        loader={<h4>Loading...</h4>}
        style={{ display: "flex", overflowX: "auto", padding: "8px 0" }}
      >
        {data &&
          data.length > 0 &&
          data.map((follower) => (
            <Card
              style={{ width: 120, marginRight: 8, textAlign: "center" }}
              key={follower.id}
            >
              <ImageAvatar url={follower.avatar} size={64} />
              <Text>{follower.displayName}</Text>
            </Card>
          ))}
      </InfiniteScroll>
    </Card>
  );
};

export default FollowersCard;

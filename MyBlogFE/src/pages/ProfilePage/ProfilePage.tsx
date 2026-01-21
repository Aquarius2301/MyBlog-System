import { accountApi, postApi } from "@/api";
import { PageLayout, ProfileLoading } from "@/components";
import { PostProvider } from "@/contexts";
import { PostList, ProfileCard } from "@/feature";
import { useApiQuery, useAuth, useSafeTranslation } from "@/hooks";
import useUrlSearchParams from "@/hooks/useUrlSearchParams";
import { Card, Result } from "antd";
import { useEffect } from "react";
import { useNavigate } from "react-router-dom";

const MyProfilePage = () => {
  const { account } = useAuth();
  const navigate = useNavigate();
  const { getValue } = useUrlSearchParams();

  const username = getValue("u");

  const canFetch = !!username && !!account && account.username !== username;

  const { isLoading, data, error } = useApiQuery({
    queryKey: ["getProfile", username],
    queryFn: () => accountApi.getProfileByUsername(username || ""),
    enabled: canFetch,
  });

  const { t } = useSafeTranslation();

  useEffect(() => {
    if (account && username && account.username === username) {
      navigate("/profile/me");
    }
  }, [account, username, navigate]);

  return (
    <PageLayout title={t("ProfilePage")}>
      {isLoading ? (
        <ProfileLoading />
      ) : data ? (
        <>
          <ProfileCard account={data} />
          <PostProvider>
            <PostList
              fetch={(cursor) =>
                postApi.getPostByUsername(data.username, cursor)
              }
              showCreatePost={false}
            />
          </PostProvider>
        </>
      ) : (
        error && (
          <Card>
            <Result title={t("ProfileNotFound")} status={"404"} />
          </Card>
        )
      )}
    </PageLayout>
  );
};

export default MyProfilePage;

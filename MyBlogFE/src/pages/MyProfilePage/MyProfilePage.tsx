import { postApi } from "@/api";
import { PageLayout, ProfileLoading } from "@/components";
import { PostProvider } from "@/contexts/PostContext";
import { PostList, ProfileCard } from "@/feature";
import { useAuth, useSafeTranslation } from "@/hooks";
import { useEffect, useState } from "react";

const MyProfilePage = () => {
  const [loading, setLoading] = useState(false);
  const { account } = useAuth();
  const { t } = useSafeTranslation();

  useEffect(() => {
    const fetchData = async () => {
      setLoading(true);
      if (!account) return;

      setLoading(false);
    };

    fetchData();
  }, [account]);
  return (
    <PageLayout title={t("MyProfilePage")}>
      {loading ? (
        <ProfileLoading />
      ) : (
        account && (
          <>
            <ProfileCard account={account} />
            <PostProvider>
              <PostList
                fetch={(cursor) =>
                  postApi.getPostByUsername(account.username, cursor)
                }
              />
            </PostProvider>
          </>
        )
      )}
    </PageLayout>
  );
};

export default MyProfilePage;

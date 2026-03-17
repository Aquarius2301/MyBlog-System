import { postApi } from "@/api";
import { PageLayout } from "@/components";
import { PostList } from "@/feature";
import { useSafeTranslation } from "@/hooks";
import { useQueryClient } from "@tanstack/react-query";
import { useEffect } from "react";

const HomePage = () => {
  const { t } = useSafeTranslation();
  const queryClient = useQueryClient();

  useEffect(() => {
    queryClient.setQueryData(["posts"], () => null);
  }, []);

  return (
    <PageLayout title={t("HomePage")}>
      <PostList fetch={postApi.getPosts} />
    </PageLayout>
  );
};

export default HomePage;

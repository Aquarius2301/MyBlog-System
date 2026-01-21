// import { postApi } from "@/api";
// import { PageLayout } from "@/components";
// import { PostItem, PostLoadingCard } from "@/feature";
// import { useSafeTranslation } from "@/hooks";

// import { useEffect, useState } from "react";
// import { useNavigate } from "react-router-dom";

// const ViewPostPage = () => {
//   const [post, setPost] = useState<GetPostDetailData
//    | null>(null);

//   const navigate = useNavigate();
//   const { t } = useSafeTranslation();
//   // const { loading, execute } = useApi<GetPostDetailData>(
//   //   postApi.getPostByLink,
//   //   {
//   //     onSuccess: ({ data }) => {
//   //       setPost(data);
//   //     },
//   //     onError: () => {
//   //       navigate("/notFound?message=PostNotFound");
//   //     },
//   //   }
//   // );

//   useEffect(() => {
//     const fetchPost = async () => {
//       const link = window.location.search.replace("?link=", "");
//       await execute(link);
//     };

//     fetchPost();
//   }, []);

//   return (
//     <PageLayout title={t("PostDetails")}>
//       {loading ? (
//         <PostLoadingCard />
//       ) : (
//         post && (
//           <PostItem
//             post={post}
//             onPostUpdated={(data) => setPost(data)}
//             onPostDeleted={() => navigate("/home")}
//           />
//         )
//       )}
//     </PageLayout>
//   );
// };
// export default ViewPostPage;

export default function ViewPostPage() {
  return <div>ViewPostPage is under construction.</div>;
}

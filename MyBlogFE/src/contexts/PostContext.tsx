import type { GetPostDetailData, GetPostsData } from "@/types/post.type";
import {
  createContext,
  useEffect,
  useRef,
  useState,
  type ReactNode,
} from "react";

interface PostContextType {
  /** The posts  */
  posts: GetPostsData[];

  /** Set the posts */
  setPost: (posts: GetPostsData) => void;

  /** Add a new post */
  addPost: (post: GetPostDetailData) => void;

  /** Update an existing post */
  updatePost: (post: GetPostDetailData) => void;

  /** Update an existing post but keep the reference*/
  updateDataPost: (post: GetPostDetailData) => void;

  /** Remove a post by ID */
  removePost: (postId: string) => void;

  /** Like a post */
  likePost: (postId: string, likeCount: number) => void;

  /** Unlike a post */
  unlikePost: (postId: string, likeCount: number) => void;

  /** Get a post by ID */
  getPostByID: (postId: string) => GetPostsData | null;

  /** Get a post by link */
  getPostByLink: (postLink: string) => GetPostsData | undefined;

  /** Increase the comment count of a post */
  increaseCommentCount: (postId: string) => void;

  /** Decrease the comment count of a post */
  decreaseCommentCount: (postId: string) => void;
}

export const PostContext = createContext<PostContextType | undefined>(
  undefined
);

export const PostProvider = ({ children }: { children: ReactNode }) => {
  const [posts, setPosts] = useState<GetPostsData[]>([]);
  const prevPostsRef = useRef<GetPostsData[]>([]);

  useEffect(() => {
    const prevPosts = prevPostsRef.current;
    // console.log("Posts updated:", posts);

    posts.forEach((post, index) => {
      const prevPost = prevPosts.find((p) => p.id === post.id);

      if (!prevPost) {
        console.log("➕ New post added:", post);
        return;
      }

      if (JSON.stringify(prevPost) !== JSON.stringify(post)) {
        console.log("✏️ Post updated:", {
          before: prevPost,
          after: post,
        });
      }
    });

    prevPosts.forEach((prev) => {
      if (!posts.find((p) => p.id === prev.id)) {
        console.log("🗑 Post removed:", prev);
      }
    });

    prevPostsRef.current = posts;
  }, [posts]);

  const getPostByLink = (postLink: string): GetPostsData | undefined => {
    const post = posts.find((p) => p.link === postLink);
    return post;
  };

  const getPostByID = (postId: string): GetPostsData | null => {
    const post = posts.find((p) => p.id === postId);
    return post || null;
  };

  const addPost = (post: GetPostDetailData) => {
    const newPost: GetPostsData = {
      ...post,
      latestComment: null,
      score: 0,
    };

    setPosts((prevPosts) => [newPost, ...prevPosts]);
  };

  const setPost = (post: GetPostsData) => {
    // setPosts((prev) => {
    //   return postsFromApi.reduce((acc, apiPost) => {
    //     const existingIndex = acc.findIndex((p) => p.id === apiPost.id);

    //     if (existingIndex !== -1) {
    //       // Update existing post
    //       acc[existingIndex] = apiPost;
    //     } else {
    //       // Add new post to the end
    //       acc.push(apiPost);
    //     }

    //     return acc;
    //   }, prev);
    // });
    console.log("Set post in context:", post.id);

    setPosts((prevPosts) => {
      let found = false;
      const updatedPosts = prevPosts.map((p) => {
        if (p.id === post.id) {
          found = true;
          return post;
        }
        return p;
      });
      if (!found) {
        updatedPosts.push(post);
      }
      return updatedPosts;
    });
  };

  const updatePost = (post: GetPostDetailData) => {
    setPosts((prevPosts) =>
      prevPosts.map((p) =>
        p.id === post.id
          ? {
              ...p,
              author: post.author,
              createdAt: post.createdAt,
              id: post.id,
              link: post.link,
              isOwner: post.isOwner,
              score: p.score,
              latestComment: p.latestComment,
              content: post.content,
              postPictures: post.postPictures,
              updatedAt: post.updatedAt,
              isLiked: post.isLiked,
              likeCount: post.likeCount,
              commentCount: post.commentCount,
            }
          : p
      )
    );
  };

  const updateDataPost = (updated: GetPostDetailData) => {
    setPosts((prevPosts) => {
      const post = prevPosts.find((p) => p.id === updated.id);
      if (post) {
        post.author = updated.author;
        post.content = updated.content;
        post.createdAt = updated.createdAt;
        post.updatedAt = updated.updatedAt;
        post.postPictures = updated.postPictures;
        post.isOwner = updated.isOwner;
        post.isLiked = updated.isLiked;
        post.likeCount = updated.likeCount;
        post.commentCount = updated.commentCount;
      }
      return [...prevPosts];
    });
  };

  const removePost = (postId: string) => {
    setPosts((prevPosts) => prevPosts.filter((post) => post.id !== postId));
  };

  // const likePost = (postId: string, likeCount: number) => {
  //   setPosts((prevPosts) =>
  //     prevPosts.map((p) =>
  //       p.id == postId ? { ...p, isLiked: true, likeCount: likeCount } : p
  //     )
  //   );
  // };

  const likePost = (postId: string, likeCount: number) => {
    setPosts((prevPosts) => {
      const index = prevPosts.findIndex((p) => p.id === postId);
      if (index === -1) return prevPosts;

      const newPosts = [...prevPosts];
      newPosts[index] = { ...newPosts[index], isLiked: true, likeCount };

      return newPosts;
    });
  };

  const unlikePost = (postId: string, likeCount: number) => {
    // setPosts((prevPosts) =>
    //   prevPosts.map((p) =>
    //     p.id == postId ? { ...p, isLiked: false, likeCount: likeCount } : p
    //   )
    // );

    setPosts((prevPosts) => {
      const index = prevPosts.findIndex((p) => p.id === postId);
      if (index === -1) return prevPosts;

      const newPosts = [...prevPosts];
      newPosts[index] = { ...newPosts[index], isLiked: false, likeCount };

      return newPosts;
    });
  };

  const increaseCommentCount = (postId: string) => {
    setPosts((prev) =>
      prev.map((post) =>
        post.id === postId
          ? { ...post, commentCount: post.commentCount + 1 }
          : post
      )
    );
  };

  const decreaseCommentCount = (postId: string) => {
    setPosts((prev) =>
      prev.map((post) =>
        post.id === postId
          ? { ...post, commentCount: post.commentCount - 1 }
          : post
      )
    );
  };

  return (
    <PostContext.Provider
      value={{
        addPost,
        posts,
        setPost,
        updatePost,
        removePost,
        likePost,
        unlikePost,
        getPostByLink,
        getPostByID,
        updateDataPost,
        increaseCommentCount,
        decreaseCommentCount,
      }}
    >
      {children}
    </PostContext.Provider>
  );
};

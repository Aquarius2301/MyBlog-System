import { usePost } from "@/hooks";
import type { GetCommentsData } from "@/types/comment.type";
import { createContext, useEffect, useState, type ReactNode } from "react";

interface CommentContextType {
  comments: GetCommentsData[];
  setComment(comments: GetCommentsData[]): void;
  addComment: (comment: GetCommentsData) => void;
  addUnderComment: (comment: GetCommentsData, parentCommentId: string) => void;
  removeComment: (commentId: string, postId: string) => void;
  likeComment: (commentId: string, likeCount: number) => void;
  unlikeComment: (commentId: string, likeCount: number) => void;
  onResetComments: () => void;
}

export const CommentContext = createContext<CommentContextType | undefined>(
  undefined
);

export const CommentProvider = ({ children }: { children: ReactNode }) => {
  const [comments, setComments] = useState<GetCommentsData[]>([]);

  const { increaseCommentCount, decreaseCommentCount, getPostByID } = usePost();

  const setComment = (commentsFromApi: GetCommentsData[]) => {
    setComments((prev) => {
      const existedIds = new Set(prev.map((p) => p.id));

      const newData = commentsFromApi.filter((p) => !existedIds.has(p.id));

      return newData.length > 0 ? [...prev, ...newData] : prev;
    });
  };

  const addComment = (comment: GetCommentsData) => {
    setComments((prevComments) => [comment, ...prevComments]);
    const post = getPostByID(comment.postId);
    if (post) increaseCommentCount(comment.postId);
  };

  const addUnderComment = (
    comment: GetCommentsData,
    parentCommentId: string
  ) => {
    console.log("Adding under comment:", comment, parentCommentId);
    setComments((prevComments) => {
      const parentIndex = prevComments.findIndex(
        (c) => c.id === parentCommentId
      );
      if (parentIndex === -1) {
        return prevComments;
      }
      const newComments = [...prevComments];
      newComments.splice(parentIndex + 1, 0, comment);
      console.log("New comments after adding under comment:", newComments);
      return newComments;
    });

    const post = getPostByID(comment.postId);
    if (post) increaseCommentCount(comment.postId);
  };

  const removeComment = (commentId: string, postId: string) => {
    setComments((prevComments) =>
      prevComments.filter((comment) => comment.id !== commentId)
    );
    decreaseCommentCount(postId);
  };

  const likeComment = (commentId: string, likeCount: number) => {
    setComments((prevComments) =>
      prevComments.map((c) =>
        c.id === commentId ? { ...c, isLiked: true, likeCount: likeCount } : c
      )
    );
  };

  const unlikeComment = (commentId: string, likeCount: number) => {
    setComments((prevComments) =>
      prevComments.map((c) =>
        c.id === commentId ? { ...c, isLiked: false, likeCount: likeCount } : c
      )
    );
  };

  const onResetComments = () => {
    setComments([]);
  };

  useEffect(() => {
    console.log("Comments updated:", comments);
  }, [comments]);

  return (
    <CommentContext.Provider
      value={{
        addUnderComment,
        comments,
        setComment,
        addComment,
        removeComment,
        likeComment,
        unlikeComment,
        onResetComments,
      }}
    >
      {children}
    </CommentContext.Provider>
  );
};

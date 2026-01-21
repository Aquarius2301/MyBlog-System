import { CommentContext } from "@/contexts";
import { useContext } from "react";

export default function useComment() {
  const context = useContext(CommentContext);
  if (!context) {
    throw new Error("useComment must be used within a CommentProvider");
  }
  return context;
}

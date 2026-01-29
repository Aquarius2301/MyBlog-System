import { useNavigate } from "react-router-dom";
import { useEffect } from "react";
import {
  RegisterConfirm,
  ResetPasswordConfirm,
} from "@/pages/ConfirmPage/components";

export default function ConfirmPage() {
  const navigate = useNavigate();

  const query = new URLSearchParams(window.location.search);
  const token = query.get("token");
  const type = query.get("type");

  useEffect(() => {
    if (!type || !token) {
      navigate("/notFound?message=TokenMissing");
    }
  }, [type, token, navigate]);

  if (type === "register" && token) {
    return <RegisterConfirm type={type} token={token} />;
  }

  if (type === "forgotPassword" && token) {
    return <ResetPasswordConfirm type={type} token={token} />;
  }

  return null;
}

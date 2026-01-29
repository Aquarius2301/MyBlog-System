// import { authApi } from "@/api";
// import { useApiQuery, useSafeTranslation, useTheme } from "@/hooks";
// import { BackgroundColor } from "@/components/style.type";
// import { Button, Card, Flex, Result, Spin } from "antd";
// import { useState } from "react";
// import { useNavigate } from "react-router-dom";

// type Props = {
//   type: string;
//   token: string;
// };

// export const RegisterConfirm = ({ type, token }: Props) => {
//   const [message, setMessage] = useState("");
//   const { themeMode } = useTheme();
//   const { t } = useSafeTranslation();
//   const navigate = useNavigate();

//   const { data, error, isLoading } = useApiQuery({
//     queryKey: ["confirm", type, token],
//     queryFn: () => authApi.confirm(type, token),
//     // onSuccess: (data) => {
//     //   if (data) {
//     //     if (data.statusCode === 200) {
//     //       setMessage(data.message);
//     //     } else {
//     //       navigate("/notFound?message=TokenMissing");
//     //     }
//     //   }
//     //   setLoading(false);
//     // },
//     enabled: type && token ? true : false,
//   });

//   // useEffect(() => {
//   //   if (error) {
//   //     navigate("/notFound?message=TokenMissing");
//   //   }
//   // }, [error, navigate]);

//   // useEffect(() => {
//   //   if (data) {
//   //     setMessage(data);
//   //   }
//   // }, [data, navigate]);

//   if (isLoading) {
//     return <Spin size="large" fullscreen />;
//   }
//   if (error) {
//     return (
//       <Card style={{ width: "100vw", height: "100vh" }}>
//         <Result title={t("TokenMissing")} status={"404"} />
//       </Card>
//     );
//   } else if (!error && data) {
//     return (
//       <Flex
//         justify="center"
//         align="center"
//         style={{
//           backgroundColor: BackgroundColor[themeMode],
//           height: "100vh",
//           width: "100vw",
//         }}
//       >
//         <Card>
//           <Result
//             status={"success"}
//             title={message}
//             extra={[
//               <Button key="login" type="primary" href="/login">
//                 {t("GotoLogin")}
//               </Button>,
//             ]}
//           />
//         </Card>
//       </Flex>
//     );
//   }

//   // useEffect(() => {
//   //   const fetchConfirm = async () => {
//   //     if (hasFetched.current) return; // Avoid run twice in StrictMode
//   //     hasFetched.current = true;

//   //     setLoading(true);

//   //     if (type && token) {
//   //       var res = await authApi.confirm(type, token);
//   //       if (res.statusCode === 200) {
//   //         setMessage(res.message);
//   //       } else {
//   //         navigate("/notFound?message=TokenMissing");
//   //       }
//   //     }
//   //     setLoading(false);
//   //   };

//   //   fetchConfirm();
//   // }, []);

//   // return loading ? (
//   //   <Spin size="large" fullscreen />
//   // ) : (
//   //   <Flex
//   //     justify="center"
//   //     align="center"
//   //     style={{
//   //       backgroundColor: BackgroundColor[themeMode],
//   //       height: "100vh",
//   //       width: "100vw",
//   //     }}
//   //   >
//   //     <Card>
//   //       <Result
//   //         status={"success"}
//   //         title={message}
//   //         extra={[
//   //           <Button key="login" type="primary" href="/login">
//   //             {t("GotoLogin")}
//   //           </Button>,
//   //         ]}
//   //       />
//   //     </Card>
//   //   </Flex>
//   // );
// };

import { authApi } from "@/api";
import { useApiQuery, useSafeTranslation, useTheme } from "@/hooks";
import { BackgroundColor } from "@/components/style.type";
import { Button, Card, Flex, Result, Spin } from "antd";

type Props = {
  type: string;
  token: string;
};

export const RegisterConfirm = ({ type, token }: Props) => {
  const { themeMode } = useTheme();
  const { t } = useSafeTranslation();

  const { data, error, isLoading } = useApiQuery({
    queryKey: ["confirm", type, token],
    queryFn: () => authApi.confirm(type, token),
    enabled: !!type && !!token,
  });

  /**
   * Loading
   */
  if (isLoading) {
    return <Spin size="large" fullscreen />;
  }

  /**
   * Token invalid / expired
   */
  if (error || data === null) {
    return (
      <Card style={{ width: "100vw", height: "100vh" }}>
        <Result status="404" title={t("TokenMissing")} />
      </Card>
    );
  }

  /**
   * Confirm success
   */
  return (
    <Flex
      justify="center"
      align="center"
      style={{
        backgroundColor: BackgroundColor[themeMode],
        height: "100vh",
        width: "100vw",
      }}
    >
      <Card>
        <Result
          status="success"
          title={data}
          extra={
            <Button type="primary" href="/login">
              {t("GotoLogin")}
            </Button>
          }
        />
      </Card>
    </Flex>
  );
};

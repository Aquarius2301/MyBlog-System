import React, { useEffect, useState } from "react";
import * as signalR from "@microsoft/signalr";
import { Row } from "antd";
import { PageLayout } from "@/components";
import ConversationList from "./components/ConversationList";
import MessageList from "./components/MessageList";

const ChatPage: React.FC = () => {
  const [connection, setConnection] = useState<signalR.HubConnection | null>(
    null,
  );

  // const queryClient = useQueryClient();
  // const [input, setInput] = useState("");

  // const { getValue } = useUrlSearchParams();
  const [receiverId, setReceiverId] = useState<string | null>(null);
  const [conversationId, setConversationId] = useState<string | null>(null);

  useEffect(() => {
    const newConnection = new signalR.HubConnectionBuilder()
      .withUrl("http://localhost:5250/chatHub")
      .withAutomaticReconnect()
      .build();

    setConnection(newConnection);
  }, []);

  // useEffect(() => {
  //   setMessages(
  //     chatData?.map((chat: { senderId: string; message: string }) => ({
  //       user: chat.senderId,
  //       message: chat.message,
  //     })) || [],
  //   );
  // }, [chatData]);

  useEffect(() => {
    if (connection) {
      connection
        .start()
        .then(() => {
          // connection.on("ReceiveMessage", (user, message) => {
          //   setMessages((prev) => [...prev, { user, message }]);
          // });
        })
        .catch((err) => console.error(err));
    }
  }, [connection]);

  return (
    <PageLayout contentCentered={false}>
      <Row gutter={16}>
        <ConversationList
          setConversationId={setConversationId}
          setReceiverId={setReceiverId}
        />
        <MessageList conversationId={conversationId} receiverId={receiverId} />
      </Row>
    </PageLayout>
  );
};

export default ChatPage;

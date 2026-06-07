import { chatApi } from "@/api";
import { ImageAvatar, Paragraph, Text } from "@/components";
import { useApiQuery, useSafeTranslation } from "@/hooks";
import { getTimeAgo } from "@/utils";
import { Card, Col, Empty, Flex, Spin } from "antd";

type ConversationListProps = {
  setConversationId: (conversationId: string) => void;
  setReceiverId: (receiverId: string) => void;
};

const ConversationList = ({
  setConversationId,
  setReceiverId,
}: ConversationListProps) => {
  const { data: conversations, isLoading } = useApiQuery({
    queryKey: ["chat"],

    queryFn: async () => chatApi.getAllConversations(),
  });

  const { tUnsafe } = useSafeTranslation();

  return (
    <Col span={8}>
      <Card
        title="Conversations"
        style={{ height: "85vh" }}
        styles={{
          body: {
            height: "90%",
            padding: "6px",
            display: "flex",
            flexDirection: "column",
            flex: 1,
            overflow: "hidden",
          },
        }}
      >
        <Flex
          vertical
          flex={1}
          style={{
            minHeight: 0,
            padding: "12px",
            display: "flex",
            flexDirection: "column",
            overflowY: "auto",
            scrollbarGutter: "stable",
          }}
        >
          {!isLoading ? (
            conversations ? (
              conversations.map((conversation) => (
                <Card
                  key={conversation.conversationId}
                  style={{ margin: "8px 0" }}
                  styles={{ body: { padding: "16px" } }}
                  hoverable
                  onClick={() => {
                    setConversationId(conversation.conversationId);
                    setReceiverId(conversation.account.id);
                  }}
                >
                  <div style={{ marginBottom: "4px" }}>
                    <ImageAvatar url={conversation.account.avatar} />
                    <Text
                      bold={
                        conversation.lastMessage != null &&
                        conversation.lastMessage.isRead
                      }
                      style={{ marginLeft: "8px" }}
                    >
                      {conversation.account.displayName}
                    </Text>
                  </div>

                  <Paragraph
                    style={{
                      fontWeight:
                        conversation.lastMessage == null ||
                        !conversation.lastMessage.isRead
                          ? "normal"
                          : "bold",
                      color:
                        conversation.lastMessage == null ||
                        !conversation.lastMessage.isRead
                          ? "grey"
                          : "inherit",
                    }}
                  >
                    {conversation.lastMessage?.content || "No messages yet"}
                  </Paragraph>

                  <Text style={{ fontSize: "12px", color: "gray" }}>
                    {conversation.lastMessage?.createdAt &&
                      getTimeAgo(
                        conversation.lastMessage?.createdAt,
                        localStorage.getItem("language") || "en",
                      )}
                  </Text>
                </Card>
              ))
            ) : (
              <Empty description="No conversations yet" />
            )
          ) : (
            <Spin spinning />
          )}
        </Flex>
      </Card>
    </Col>
  );
};

export default ConversationList;

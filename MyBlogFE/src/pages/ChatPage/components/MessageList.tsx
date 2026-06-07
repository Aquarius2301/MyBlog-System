import { Button, Card, Col, Divider, Empty, Flex, Input, Spin } from "antd";
import { useEffect, useRef, useState } from "react";
import { SendOutlined } from "@ant-design/icons";
import { useApiInfiniteQuery } from "@/hooks";
import { chatApi } from "@/api";
import { formatDateTime } from "@/utils";

type MessageListProps = {
  conversationId: string | null;
  receiverId: string | null;
};

const MessageList = ({ conversationId, receiverId }: MessageListProps) => {
  const containerRef = useRef<HTMLDivElement | null>(null);
  const [cursor, setCursor] = useState<string | null>(null);

  useEffect(() => {
    // Always go to the last message when the component mounts
    const el = containerRef.current;
    if (el) {
      el.scrollTop = el.scrollHeight;
    }
  }, []);

  const {
    data: messages,
    isLoading,
    hasNextPage,
    fetchNextPage,
  } = useApiInfiniteQuery({
    queryKey: ["chat", conversationId, receiverId],
    queryFn: (cursor) => chatApi.getMessages(conversationId ?? "", cursor),
    enabled: conversationId != null && receiverId != null,
    // if your API returns a cursor in the response, you can map it here:
    // getNextPageParam: (lastPage) => lastPage?.cursor ?? null,
    initialPageParam: null,
  });

  useEffect(() => {
    conversationId && fetchNextPage();
  }, [conversationId]);

  return (
    <Col span={16}>
      <Card
        title="Messages (User 1)"
        style={{ height: "85vh" }}
        styles={{
          body: {
            padding: "6px",
            height: "90%",
            display: "flex",
            flexDirection: "column",
            flex: 1,
            overflow: "hidden",
          },
        }}
      >
        {isLoading ? (
          <Spin />
        ) : !conversationId ? (
          <Empty
            style={{ textAlign: "center", padding: "12px" }}
            description="Please select a conversation to view messages."
          />
        ) : (
          <>
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
              ref={containerRef}
            >
              {messages &&
                [...messages].reverse().map((message) => (
                  <Card
                    key={message.id}
                    style={{
                      marginBottom: "16px",
                      backgroundColor:
                        receiverId === message.senderId
                          ? "#3370d2ff"
                          : "inherit",
                      maxWidth: "70%",
                      alignSelf:
                        receiverId === message.senderId
                          ? "flex-start"
                          : "flex-end",
                      color:
                        receiverId === message.senderId ? "white" : "inherit",
                    }}
                    styles={{
                      body: {
                        margin: "0px",
                        padding: "12px",
                      },
                    }}
                  >
                    {/* <div style={{ fontWeight: "bold" }}>{message.user}</div> */}
                    <div>{message.content}</div>
                    <div style={{ fontSize: "12px", color: "#ccc8c8ff" }}>
                      {formatDateTime(message.createdAt)}
                    </div>
                  </Card>
                ))}
            </Flex>
            <div>
              <Divider style={{ margin: "12px 0" }} />
              <Flex
                align="flex-end"
                gap={8}
                style={{
                  padding: "8px",
                }}
              >
                <Input.TextArea
                  placeholder="Type your message..."
                  autoSize={{ minRows: 1, maxRows: 4 }}
                  style={{
                    borderRadius: "6px",
                    resize: "none",
                  }}
                />

                <Button
                  type="primary"
                  shape="circle"
                  icon={<SendOutlined />}
                  style={{
                    display: "flex",
                    alignItems: "center",
                    justifyContent: "center",
                  }}
                />
              </Flex>
            </div>
          </>
        )}
      </Card>
    </Col>
  );
};

export default MessageList;

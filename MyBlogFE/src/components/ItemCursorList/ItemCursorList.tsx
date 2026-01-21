import { Empty, Spin } from "antd";
import {
  useCallback,
  useEffect,
  useImperativeHandle,
  useRef,
  useState,
  forwardRef,
} from "react";

type ItemCursorListProps = {
  fetch: (...args: any[]) => Promise<any>;
  fetchParams?: any[];
  children: (item: any) => React.ReactNode;
  loadingItem?: React.ReactNode;
  emptyString?: string | null;
};

export type ItemCursorListRef = {
  addItem: (item: any, position?: "start" | "end") => void;
  updateItem: (id: any, updatedItem: any) => void;
  removeItem: (id: any) => void;
  prependItem: (item: any) => void;
  appendItem: (item: any) => void;
  getData: () => any[];
  refresh: () => void;
};

/**
 * @deprecated
 */
const ItemCursorList = forwardRef<ItemCursorListRef, ItemCursorListProps>(
  ({ fetch, fetchParams, children, loadingItem, emptyString = null }, ref) => {
    const [data, setData] = useState<any[]>([]);
    const [isLoading, setLoading] = useState<boolean>(false);
    const [hasMore, setHasMore] = useState<boolean>(true);

    const [currentCursor, setCurrentCursor] = useState<string | null>(null);

    const nextCursorRef = useRef<string | null>(null);
    const observer = useRef<IntersectionObserver | null>(null);
    const isFetchingRef = useRef<boolean>(false);

    // Expose methods to parent component
    useImperativeHandle(
      ref,
      () => ({
        // Add new item, position: "start" | "end"
        addItem: (item: any, position: "start" | "end" = "start") => {
          setData((prev) => {
            // Kiểm tra trùng ID
            const exists = prev.some(
              (existingItem) => existingItem.id === item.id
            );
            if (exists) {
              // Nếu đã tồn tại, update luôn
              return prev.map((existingItem) =>
                existingItem.id === item.id ? item : existingItem
              );
            }
            // Nếu chưa tồn tại, thêm mới
            return position === "start" ? [item, ...prev] : [...prev, item];
          });
        },

        // Add new item to the start of the list
        prependItem: (item: any) => {
          setData((prev) => {
            const exists = prev.some(
              (existingItem) => existingItem.id === item.id
            );
            if (exists) return prev;
            return [item, ...prev];
          });
        },

        // Add new item to the end of the list
        appendItem: (item: any) => {
          setData((prev) => {
            const exists = prev.some(
              (existingItem) => existingItem.id === item.id
            );
            if (exists) return prev;
            return [...prev, item];
          });
        },

        // Update item by ID
        updateItem: (id: any, updatedItem: any) => {
          setData((prev) =>
            prev.map((item) =>
              item.id === id ? { ...item, ...updatedItem } : item
            )
          );
        },

        // Remove item by ID
        removeItem: (id: any) => {
          setData((prev) => prev.filter((item) => item.id !== id));
        },

        // Get the current data
        getData: () => data,

        // Refresh from the beginning
        refresh: () => {
          setData([]);
          setCurrentCursor(null);
          nextCursorRef.current = null;
          setHasMore(true);
        },
      }),
      [data]
    );

    useEffect(() => {
      const fetchPosts = async () => {
        if (isFetchingRef.current) return;

        isFetchingRef.current = true;
        setLoading(true);

        try {
          const res = await fetch(...(fetchParams || []), currentCursor);

          if (res.statusCode === 200 && res.data) {
            const newPosts = res.data.items;
            const nextCursorFromApi = res.data.cursor;

            setData((prev: any[]) => {
              const rawList =
                currentCursor === null
                  ? [...newPosts, ...prev]
                  : [...prev, ...newPosts];

              const uniqueList = Array.from(
                new Map(rawList.map((item: any) => [item.id, item])).values()
              );

              return uniqueList;
            });

            nextCursorRef.current = nextCursorFromApi;

            if (!nextCursorFromApi || newPosts.length === 0) {
              setHasMore(false);
            }
          }
        } catch (error) {
          console.error("Error fetching posts:", error);
        } finally {
          setLoading(false);
          isFetchingRef.current = false;
        }
      };

      fetchPosts();
    }, [currentCursor, fetch, fetchParams]);

    const lastPostElementRef = useCallback(
      (node: HTMLDivElement) => {
        if (isLoading || isFetchingRef.current) return;

        if (observer.current) observer.current.disconnect();

        observer.current = new IntersectionObserver((entries) => {
          if (entries[0].isIntersecting && hasMore) {
            if (nextCursorRef.current) {
              setCurrentCursor((prev) => {
                if (prev !== nextCursorRef.current) {
                  return nextCursorRef.current;
                }
                return prev;
              });
            }
          }
        });

        if (node) observer.current.observe(node);
      },
      [isLoading, hasMore]
    );

    return (
      <>
        {data.length === 0 && !isLoading && (
          <Empty description={emptyString ?? "No data"} />
        )}
        {data.map((item: any, index: number) => {
          if (data.length === index + 1) {
            return (
              <div ref={lastPostElementRef} key={item.id}>
                {children(item)}
              </div>
            );
          } else {
            return <div key={item.id}>{children(item)}</div>;
          }
        })}
        {isLoading &&
          (loadingItem || (
            <Spin style={{ width: "100%", marginBottom: "20px" }} />
          ))}
      </>
    );
  }
);

// MBCursorList.displayName = "MBCursorList";

export default ItemCursorList;

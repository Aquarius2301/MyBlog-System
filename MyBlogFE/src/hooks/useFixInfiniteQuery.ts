import type { ApiResponse, PaginationResponse } from "@/types/common.type";
import { useQueryClient, type InfiniteData } from "@tanstack/react-query";

/**
 * Type alias for infinite query data structure with pagination
 * @template T - The type of individual items in the paginated data
 */
type TInfiniteData<T> = InfiniteData<
  ApiResponse<PaginationResponse<T>>,
  unknown
>;

/**
 * Props for the useFixInfiniteQuery hook
 * @template TData - The type of data items in the infinite query
 */
interface UseFixInfiniteQueryProps<TData> {
  /**
   * Function to extract a unique key from an item for identification
   * This key is used to locate and update specific items in the infinite query cache
   *
   * @param item - The data item to extract the key from
   * @returns The unique identifier for the item
   *
   * @example
   * keySelector: (item) => item.id  // For items with 'id' property
   *
   * @example
   * keySelector: (item) => item.username  // For items with 'username' property
   */
  keySelector?: (item: TData) => any;
}

/**
 * Return type for the useFixInfiniteQuery hook
 * @template TData - The type of data items in the infinite query
 */
interface UseFixInfiniteQueryReturn<TData> {
  addItem: (queryKey: any[], item: TData, position?: "start" | "end") => void;

  removeItem: (queryKey: any[], key: any) => void;

  /**
   * Updates a specific item in the infinite query cache
   *
   * @param queryKey - The query key identifying the infinite query to update
   * @param key - The unique key of the item to update (extracted via keySelector)
   * @param updater - Function that receives the old item and returns the updated item
   *
   * @example
   * updateItem(['posts'], postId, (oldPost) => ({ ...oldPost, likes: oldPost.likes + 1 }));
   */
  updateItem: (
    queryKey: any[],
    key: any,
    updater: (old: TData) => TData
  ) => void;
}

/**
 * Custom hook for manipulating items within an infinite query cache.
 *
 * This hook provides utilities to update, add, or remove items from react-query's
 * infinite query data without refetching. Useful for optimistic updates or reflecting
 * mutations in cached infinite scroll data.
 *
 * Key features:
 * - Updates items across all pages in the infinite query
 * - Uses a key selector to identify items uniquely
 * - Preserves pagination structure and cursor information
 * - Type-safe operations on cached data
 *
 * @template TData - The type of individual data items in the infinite query
 *
 * @param {UseFixInfiniteQueryProps<TData>} props - Configuration object
 * @param {(item: TData) => any} [props.keySelector] - Function to extract unique key from items
 *
 * @returns {UseFixInfiniteQueryReturn<TData>} Object containing cache manipulation methods
 *
 * @example
 * // Setup the hook with a key selector
 * const { updateItem } = useFixInfiniteQuery<Post>({
 *   keySelector: (post) => post.id,
 * });
 *
 * // Update a specific post after liking it
 * const handleLike = (postId: string) => {
 *   updateItem(['posts'], postId, (oldPost) => ({
 *     ...oldPost,
 *     likes: oldPost.likes + 1,
 *     isLiked: true,
 *   }));
 * };
 *
 * @example
 * // Update user profile in infinite followers list
 * const { updateItem } = useFixInfiniteQuery<User>({
 *   keySelector: (user) => user.username,
 * });
 *
 * updateItem(['followers'], 'john_doe', (oldUser) => ({
 *   ...oldUser,
 *   isFollowing: true,
 * }));
 *
 * @todo Add addItem function to prepend/append items to the cache
 * @todo Add removeItem function to remove items from the cache
 * @todo Add support for batch updates to modify multiple items at once
 */
export default function useFixInfiniteQuery<TData>({
  keySelector,
}: UseFixInfiniteQueryProps<TData>): UseFixInfiniteQueryReturn<TData> {
  const queryClient = useQueryClient();

  /**
   * Updates a single item across all pages in the infinite query cache
   *
   * The function:
   * 1. Locates the item using the provided key and keySelector
   * 2. Applies the updater function to transform the item
   * 3. Preserves all other items and pagination structure
   * 4. Returns early if keySelector is not provided
   *
   * @param queryKey - React Query key identifying the infinite query
   * @param key - The unique identifier of the item to update
   * @param updater - Transformation function that receives old item and returns new item
   */
  const updateItem = (
    queryKey: any[],
    key: any,
    updater: (old: TData) => TData
  ) => {
    // Early return if no key selector is provided
    if (!keySelector) return;

    // Update the query cache immutably
    queryClient.setQueryData(queryKey, (old: TInfiniteData<TData>) => {
      // Return unchanged if no data exists
      if (!old) return old;

      return {
        ...old,
        // Map through all pages in the infinite query
        pages: old.pages.map(
          (page: ApiResponse<PaginationResponse<TData>>) => ({
            ...page,
            data: {
              ...page.data,
              // Map through items in each page and apply updater to matching item
              items: page.data?.items.map((item: TData) =>
                keySelector(item) === key ? updater(item) : item
              ),
            },
          })
        ),
      };
    });
  };

  const addItem = (
    queryKey: any[],
    item: TData,
    position: "start" | "end" = "start"
  ) => {
    queryClient.setQueryData(queryKey, (old: TInfiniteData<TData>) => {
      if (!old) return old;

      return {
        ...old,
        pages: old.pages.map(
          (page: ApiResponse<PaginationResponse<TData>>, index) => {
            // Add the item only to the first page
            if (index === 0) {
              return {
                ...page,
                data: {
                  ...page.data,
                  items:
                    position === "start"
                      ? [item, ...(page.data?.items ?? [])]
                      : [...(page.data?.items ?? []), item],
                },
              };
            }
            return page;
          }
        ),
      };
    });
  };

  const removeItem = (queryKey: any[], key: any) => {
    if (!keySelector) return;

    queryClient.setQueryData(queryKey, (old: TInfiniteData<TData>) => {
      if (!old) return old;
      return {
        ...old,
        pages: old.pages.map(
          (page: ApiResponse<PaginationResponse<TData>>) => ({
            ...page,
            data: {
              ...page.data,
              items: page.data?.items.filter(
                (item: TData) => keySelector(item) !== key
              ),
            },
          })
        ),
      };
    });
  };

  return { updateItem, addItem, removeItem };
}

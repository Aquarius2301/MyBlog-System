import type { ApiResponse, PaginationResponse } from "@/types/common.type";
import { useInfiniteQuery } from "@tanstack/react-query";
import axios from "axios";
import { useEffect } from "react";
import useSafeTranslation from "./useSafeTranslation";
import { App } from "antd";

/**
 * Props for the useApiInfiniteQuery hook
 * @template TData - The type of individual items in the paginated response
 * @template TVariables - The type of variables/parameters passed to each page query
 */
interface UseApiInfiniteQueryOptions<TData, TVariables> {
  /** The query key used by react-query to identify and cache the infinite query */
  queryKey: any[];

  /**
   * The function that performs the API query for each page
   * Receives pageParam (cursor/offset) via variables and returns paginated response
   */
  queryFn: (
    pageParam: TVariables
  ) => Promise<ApiResponse<PaginationResponse<TData>>>;

  /**
   * The initial page parameter for the first query
   * Typically null, 0, or an initial cursor value
   */
  initialPageParam?: any;

  /** Whether the query should be enabled or not. Defaults to true */
  enabled?: boolean;
}

/**
 * Return type for the useApiInfiniteQuery hook
 * @template TData - The type of individual items in the paginated data
 */
interface UseApiInfiniteQueryReturn<TData> {
  /**
   * Flattened array of all items from all pages, with duplicates removed
   * Null if no data has been fetched yet or fetching failed
   */
  data: TData[] | null | undefined;

  /** The error object if the query failed */
  error: any;

  /** Indicates whether the initial query is currently loading */
  isLoading: boolean;

  /**
   * Function to fetch the next page of data
   * Uses the cursor from the last page to fetch subsequent data
   */
  fetchNextPage: () => void;

  /**
   * Indicates whether there is a next page to fetch
   * Determined by react-query based on getNextPageParam result
   */
  hasNextPage: boolean;

  /** Indicates whether the next page is currently being fetched */
  isFetchingNextPage: boolean;
}

/**
 * Custom hook for performing infinite scroll/pagination queries with built-in error handling.
 *
 * This hook wraps react-query's useInfiniteQuery and provides:
 * - Automatic extraction and flattening of paginated data from ApiResponse
 * - Deduplication of items across pages based on 'id' property
 * - Smart retry logic (up to 3 attempts with exponential backoff)
 * - Special handling for 401 (no retry) and 500 (error notification) status codes
 * - Cursor-based pagination support
 * - Automatic error notifications for internal server errors
 *
 * The hook automatically manages pagination state and provides a flattened array
 * of all items across all loaded pages, making it easy to implement infinite scroll UIs.
 *
 * @template TData - The type of individual items in the paginated response
 * @template TVariables - The type of variables/parameters (including pageParam) for queries
 *
 * @param {UseApiInfiniteQueryOptions<TData, TVariables>} props - Configuration object
 * @param {any[]} props.queryKey - Unique key for query identification and caching
 * @param {Function} props.queryFn - Function that fetches a page of data
 * @param {any} [props.initialPageParam] - Initial cursor/offset for the first page
 * @param {boolean} [props.enabled=true] - Whether the query should run automatically
 *
 * @returns {UseApiInfiniteQueryReturn<TData>} Object containing paginated data and control methods
 *
 * @example
 * // Basic infinite scroll for posts
 * const {
 *   data: posts,
 *   isLoading,
 *   fetchNextPage,
 *   hasMore,
 *   isFetchingNextPage
 * } = useApiInfiniteQuery({
 *   queryKey: ['posts'],
 *   queryFn: ({ variables }) => api.getPosts(variables),
 *   initialPageParam: null,
 * });
 *
 * // Render posts and load more button
 * return (
 *   <>
 *     {posts.map(post => <PostCard key={post.id} post={post} />)}
 *     {hasMore && (
 *       <button onClick={fetchNextPage} disabled={isFetchingNextPage}>
 *         {isFetchingNextPage ? 'Loading...' : 'Load More'}
 *       </button>
 *     )}
 *   </>
 * );
 *
 * @example
 * // With filters and conditional fetching
 * const [filters, setFilters] = useState({ category: 'tech' });
 *
 * const { data: articles, fetchNextPage } = useApiInfiniteQuery({
 *   queryKey: ['articles', filters],
 *   queryFn: ({ variables }) => api.getArticles({ ...filters, ...variables }),
 *   initialPageParam: { cursor: null },
 *   enabled: !!filters.category, // Only fetch when category is selected
 * });
 */
export default function useApiInfiniteQuery<TData = any, TVariables = any>({
  queryKey,
  queryFn,
  initialPageParam,
  enabled = true,
}: UseApiInfiniteQueryOptions<
  TData,
  TVariables
>): UseApiInfiniteQueryReturn<TData> {
  const { message } = App.useApp();
  const { t } = useSafeTranslation();

  const query = useInfiniteQuery({
    queryKey,
    queryFn: ({ pageParam }: { pageParam: TVariables }) => queryFn(pageParam),

    /**
     * Determines the parameter for the next page
     * Returns the cursor from the last page, or null if no more pages exist
     */
    getNextPageParam: (lastPage) => lastPage.data?.cursor || null,

    initialPageParam,
    enabled,

    /**
     * Custom retry logic for failed requests
     * - 401 Unauthorized: No retry (authentication required)
     * - Other errors: Retry up to 3 times
     */
    retry(failureCount, error) {
      // Don't retry on 401 Unauthorized errors
      if (axios.isAxiosError(error)) {
        if (error.response?.status === 401) {
          return false;
        }
      }

      // Retry up to 3 times for other errors
      return failureCount < 3;
    },

    /**
     * Exponential backoff strategy for retries
     * Delay = min(1000 * 2^attemptIndex, 30000)ms
     * - 1st retry: 2s
     * - 2nd retry: 4s
     * - 3rd retry: 8s
     */
    retryDelay(attemptIndex) {
      return Math.min(1000 * 2 ** attemptIndex, 30000);
    },
  });

  /**
   * Effect to handle error notifications
   * Shows a user-friendly error message for 500 Internal Server Error
   * Only triggers once per error to avoid duplicate notifications
   */
  useEffect(() => {
    if (query.isError) {
      if (axios.isAxiosError(query.error) && query.error.response) {
        const errorResponse = query.error;

        // Display error notification for 500 Internal Server Error
        if (errorResponse.response?.status === 500) {
          message.error(t("UnresolvedError"));
          return;
        }
      }
    }
  }, [query.isError, query.error, message, t]);

  /**
   * Flatten and deduplicate data from all pages
   *
   * Process:
   * 1. Flatten all items from all pages into a single array
   * 2. Extract unique IDs using Set to remove duplicates
   * 3. Map IDs back to original items (preserving first occurrence)
   * 4. Filter out any null/undefined values
   *
   * This ensures no duplicate items appear in the final array,
   * which can happen if pages overlap or data changes during pagination
   */
  const data =
    query.data === undefined
      ? undefined
      : query.data
      ? (Array.from(
          new Set(
            query.data.pages
              .flatMap((p) => p.data?.items)
              .map((item: any) => item.id)
          )
        )
          .map((id) =>
            query.data.pages
              .flatMap((p) => p.data?.items)
              .find((item: any) => item.id === id)
          )
          .filter(Boolean) as TData[])
      : null;

  return {
    // Flattened and deduplicated items from all pages
    data: data,
    error: query.error,
    isLoading: query.isLoading,
    fetchNextPage: query.fetchNextPage,
    hasNextPage: query.hasNextPage,
    isFetchingNextPage: query.isFetchingNextPage,
  };
}

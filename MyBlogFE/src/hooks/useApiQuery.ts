import type { ApiResponse } from "@/types/common.type";
import { useQuery, type QueryObserverResult } from "@tanstack/react-query";
import { App } from "antd";
import axios from "axios";
import { useEffect } from "react";
import useSafeTranslation from "./useSafeTranslation";

/**
 * Props for the useApiQuery hook
 * @template T - The type of data contained in the API response
 */
interface UseApiQueryProps<T> {
  /** The query key used by react-query to identify and cache the query */
  queryKey: readonly unknown[];

  /** The function that performs the API call and returns a promise with ApiResponse */
  queryFn: () => Promise<ApiResponse<T>>;

  /** Whether the query should be enabled or not. Defaults to true */
  enabled?: boolean;
}

/**
 * Return type for the useApiQuery hook
 * @template T - The type of data returned by the API
 */
interface UseApiQueryReturn<T> {
  /** The extracted data from ApiResponse, or null if not available */
  data: T | null | undefined;

  /** The error object if the query failed */
  error: ApiResponse<unknown>;

  /** Whether the query is currently loading */
  isLoading: boolean;

  /** Whether the query is currently fetching (including background refetches) */
  isFetching: boolean;

  /** Function to manually refetch the data */
  refetch: () => Promise<QueryObserverResult<ApiResponse<T>, Error>>;
}

/**
 * Custom hook for performing API queries with built-in error handling and retry logic.
 *
 * This hook wraps react-query's useQuery and provides:
 * - Automatic extraction of data from ApiResponse<T>
 * - Smart retry logic (up to 3 attempts with exponential backoff)
 * - Special handling for 401 (no retry) and 500 (error message) status codes
 * - Automatic error notifications for internal server errors
 *
 * @template T - The type of data returned by the API (inside ApiResponse<T>)
 *
 * @param {UseApiQueryProps<T>} props - Configuration object for the query
 * @param {readonly unknown[]} props.queryKey - Unique key for query identification and caching
 * @param {() => Promise<ApiResponse<T>>} props.queryFn - Function that performs the API call
 * @param {boolean} [props.enabled=true] - Whether the query should run automatically
 *
 * @returns {UseApiQueryReturn<T>} Object containing query state and methods
 *
 * @example
 * // Basic usage
 * const { data, error, isLoading, refetch } = useApiQuery({
 *   queryKey: ['users', userId],
 *   queryFn: () => api.getUser(userId),
 * });
 *
 * @example
 * // With conditional fetching
 * const { data, isLoading } = useApiQuery({
 *   queryKey: ['profile'],
 *   queryFn: () => api.getProfile(),
 *   enabled: isAuthenticated,
 * });
 */
export default function useApiQuery<T = any>({
  queryKey,
  queryFn,
  enabled = true,
}: UseApiQueryProps<T>): UseApiQueryReturn<T> {
  const { message } = App.useApp();
  const { t } = useSafeTranslation();

  const query = useQuery<ApiResponse<T>>({
    queryKey,
    queryFn,
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

        return errorResponse.response?.data;
      }
    }
  }, [query.isError, query.error, message, t]);

  return {
    // Extract data from ApiResponse wrapper, default to null if unavailable
    data: query.data?.data,
    error:
      (axios.isAxiosError(query.error) &&
        query.error.response &&
        query.error.response.data) ||
      null,
    isLoading: query.isLoading,
    refetch: query.refetch,
    isFetching: query.isFetching,
  };
}

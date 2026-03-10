import type { ApiResponse } from "@/types/common.type";
import { useMutation } from "@tanstack/react-query";
import { App } from "antd";
import axios from "axios";
import useSafeTranslation from "./useSafeTranslation";

/**
 * Props for the useApiMutation hook
 * @template TData - The type of data contained in the API response
 * @template TVariables - The type of variables/payload passed to the mutation
 */
interface UseApiMutationProps<TData, TVariables> {
  /** Optional mutation key used by react-query to identify and track the mutation */
  mutationKey?: any[];

  /** The function that performs the API mutation and returns a promise with ApiResponse */
  mutationFn: (variables: TVariables) => Promise<ApiResponse<TData>>;

  /** Callback function invoked on successful API response with extracted data */
  onSuccess?: (data: TData | null) => void;

  /** Callback function invoked on API error with error response data */
  onError?: (error: ApiResponse<any>) => void;
}

/**
 * Return type for the useApiMutation hook
 * @template TData - The type of data returned by the API
 * @template TVariables - The type of variables/payload for the mutation
 */
interface UseApiMutationReturn<TData, TVariables> {
  /** The extracted data from ApiResponse, or null if not available */
  data: TData | null;

  /** The error object if the mutation failed */
  error: any;

  /**
   * Indicates whether the mutation is currently in progress
   * Note: In Tanstack Query v5, isLoading is renamed to isPending
   */
  isLoading: boolean;

  /**
   * Function to trigger the mutation with the provided variables
   * Fire-and-forget style, use callbacks for handling response
   */
  mutate: (variables: TVariables) => void;

  /**
   * Function to trigger the mutation asynchronously with the provided variables
   * Returns a Promise for await-style handling
   * @returns Promise resolving to the complete ApiResponse
   *
   * @example
   * const response = await mutateAsync(variables);
   * console.log(response.data);
   */
  mutateAsync: (variables: TVariables) => Promise<ApiResponse<TData>>;
}

/**
 * Custom hook for performing API mutations with built-in error handling and retry logic.
 *
 * This hook wraps react-query's useMutation and provides:
 * - Automatic extraction of data from ApiResponse<T>
 * - Smart retry logic (up to 3 attempts with exponential backoff)
 * - Special handling for 401 (no retry) and 500 (error notification) status codes
 * - Automatic error notifications for internal server errors
 * - Both callback-based (mutate) and Promise-based (mutateAsync) APIs
 *
 * @template TData - The type of data returned by the API (inside ApiResponse<TData>)
 * @template TVariables - The type of variables/payload passed to the mutation function
 *
 * @param {UseApiMutationProps<TData, TVariables>} props - Configuration object for the mutation
 * @param {any[]} [props.mutationKey] - Optional unique key for mutation identification
 * @param {(variables: TVariables) => Promise<ApiResponse<TData>>} props.mutationFn - Function that performs the API call
 * @param {(data: TData | null) => void} [props.onSuccess] - Callback invoked on successful mutation
 * @param {(error: ApiResponse<any>) => void} [props.onError] - Callback invoked on mutation error
 *
 * @returns {UseApiMutationReturn<TData, TVariables>} Object containing mutation state and trigger methods
 *
 * @example
 * // Basic usage with callbacks
 * const { mutate, isLoading } = useApiMutation({
 *   mutationFn: (data) => api.createUser(data),
 *   onSuccess: (user) => {
 *     console.log('User created:', user);
 *   },
 *   onError: (error) => {
 *     console.error('Failed:', error);
 *   },
 * });
 *
 * // Trigger mutation
 * mutate({ name: 'John', email: 'john@example.com' });
 *
 * @example
 * // Using async/await style
 * const { mutateAsync, isLoading } = useApiMutation({
 *   mutationFn: (data) => api.updateProfile(data),
 * });
 *
 * const handleSubmit = async (formData) => {
 *   try {
 *     const response = await mutateAsync(formData);
 *     console.log('Profile updated:', response.data);
 *   } catch (error) {
 *     console.error('Update failed:', error);
 *   }
 * };
 */
export default function useApiMutation<TData = any, TVariables = any>({
  mutationKey,
  mutationFn,
  onSuccess,
  onError,
}: UseApiMutationProps<TData, TVariables>): UseApiMutationReturn<
  TData,
  TVariables
> {
  const { message } = App.useApp();
  const { t } = useSafeTranslation();

  const mutation = useMutation({
    mutationKey,
    mutationFn: mutationFn,

    /**
     * Success handler - extracts data from ApiResponse and invokes user callback
     */
    onSuccess: (response) => {
      onSuccess?.(response.data);
    },

    /**
     * Error handler - manages different error scenarios
     * - 500 Internal Server Error: Shows error notification
     * - Other errors: Invokes user error callback with response data
     */
    onError: (error: any) => {
      if (axios.isAxiosError(error) && error.response) {
        const errorResponse = error.response;

        // Display error notification for 500 Internal Server Error
        if (errorResponse.status === 500) {
          message.error(t("UnresolvedError"));
          return;
        }

        // Pass error response data to user callback
        onError?.(errorResponse.data);
      }
    },

    /**
     * Custom retry logic for failed mutations
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

  return {
    // Extract data from ApiResponse wrapper, default to null if unavailable
    data: mutation.data?.data || null,
    error: mutation.error,
    // Map isPending to isLoading for backwards compatibility
    isLoading: mutation.isPending,
    mutate: mutation.mutate,
    mutateAsync: mutation.mutateAsync,
  };
}

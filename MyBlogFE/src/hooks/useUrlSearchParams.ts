interface UrlSearchParamsProps {
  /** Get the value of a specific query parameter by key */
  getValue: (key: string) => string | null;

  /** Set the value of a specific query parameter by key
   * If the key already exists, its value will be updated; otherwise, a new key-value pair will be added.
   */
  setValue: (key: string, value: string) => void;
}

/** A custom hook to manage URL search parameters
 *
 * @example
 * //URL: http://example.com?page=1&username=johndoe
 * const { getValue, setValue } = useUrlSearchParams();
 * const username = getValue("username"); // returns "johndoe"
 * setValue("page", "2"); // updates the URL to http://example.com?page=2&username=johndoe
 * setValue("id", "1"); // updates the URL to http://example.com?page=1&id=1&username=johndoe
 */
export default function useUrlSearchParams(): UrlSearchParamsProps {
  const queryParams = new URLSearchParams(window.location.search);

  const getValue = (key: string): string | null => {
    return queryParams.get(key);
  };

  const setValue = (key: string, value: string): void => {
    queryParams.set(key, value);
    const newUrl = window.location.pathname + "?" + queryParams.toString();
    window.history.replaceState({}, "", newUrl);
  };

  return { getValue, setValue };
}

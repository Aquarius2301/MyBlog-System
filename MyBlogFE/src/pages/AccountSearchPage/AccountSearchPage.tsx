import { accountApi } from "@/api";
import { PageLayout, ProfileLoading } from "@/components";
import { useApiInfiniteQuery, useSafeTranslation } from "@/hooks";
import { Card, Input } from "antd";
import { useEffect, useRef, useState } from "react";
import { useSearchParams } from "react-router-dom";
import { AccountItem } from "./components/AccountItem";
import InfiniteScroll from "react-infinite-scroll-component";
import type { AccountNameData } from "@/types/account.type";

const AccountSearchPage = () => {
  const [searchParams, setSearchParams] = useSearchParams();

  const [searchValue, setSearchValue] = useState<string>(
    searchParams.get("q") || "",
  );

  const [hasSearched, setHasSearched] = useState(false);
  const timeoutRef = useRef<ReturnType<typeof setTimeout> | null>(null);

  const { t } = useSafeTranslation();

  const { data, fetchNextPage, hasNextPage } = useApiInfiniteQuery<
    AccountNameData,
    string | null
  >({
    queryKey: ["accountSearch", searchValue],
    queryFn: (pageParam) => accountApi.searchAccounts(searchValue, pageParam),
    initialPageParam: null,
    enabled: searchValue.trim().length > 0,
  });

  const handleSearchChange = (value: string) => {
    setSearchValue(value);
    setHasSearched(false);

    if (timeoutRef.current) {
      clearTimeout(timeoutRef.current);
    }

    timeoutRef.current = setTimeout(() => {
      if (value.trim()) {
        setSearchParams({ q: value.trim() });
      } else {
        setSearchParams({});
      }
      setHasSearched(true);
    }, 1000); // 1 second debounce
  };

  useEffect(() => {
    if (searchParams.get("q")?.trim()) {
      setHasSearched(true);
    }
  }, []);

  return (
    <PageLayout title={t("AccountSearch")}>
      <Card>
        <Input
          placeholder={t("UsernameOrDisplayname")}
          value={searchValue}
          onChange={(e) => handleSearchChange(e.target.value)}
          allowClear
        />
        {hasSearched && searchParams.get("q")?.trim() && (
          <div style={{ marginTop: 16 }}>
            <InfiniteScroll
              hasMore={!!hasNextPage}
              next={() => fetchNextPage()}
              loader={<ProfileLoading />}
              dataLength={data ? data.length : 0}
              endMessage={
                <p style={{ textAlign: "center" }}>
                  <b>{t("NoMoreAccounts")}</b>
                </p>
              }
            >
              {data &&
                data.map((account) => (
                  <AccountItem account={account} key={account.id} />
                ))}
            </InfiniteScroll>
          </div>
        )}
      </Card>
    </PageLayout>
  );
};

export default AccountSearchPage;

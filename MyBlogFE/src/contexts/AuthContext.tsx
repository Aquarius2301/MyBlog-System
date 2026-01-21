import { accountApi, authApi } from "@/api";
import { useApiMutation, useApiQuery } from "@/hooks";
import type { AccountData } from "@/types/account.type";
import { useQueryClient } from "@tanstack/react-query";
import { createContext, type ReactNode } from "react";

interface AuthContextReturn {
  account: AccountData | null | undefined;
  isLoading: boolean;
  logout: () => Promise<void>;
  fetchInfo: () => Promise<void>;
  //   changeLanguage: (language: string) => void;
  changeAvatar: (avatarUrl: string) => void;
}

export const AuthContext = createContext<AuthContextReturn | undefined>(
  undefined
);

export const AuthProvider = ({ children }: { children: ReactNode }) => {
  const queryClient = useQueryClient();

  const {
    data: account,
    isLoading: isProfileLoading,
    refetch: fetchProfile,
  } = useApiQuery<AccountData>({
    queryKey: ["myProfile"],
    queryFn: accountApi.getMyProfile,
    enabled: false,
  });

  const { isLoading: isLogoutLoading, mutate: fetchLogout } = useApiMutation({
    mutationFn: authApi.logout,
    mutationKey: ["logout"],
    onSuccess: () => {
      queryClient.removeQueries({ queryKey: ["myProfile"] });
    },
  });

  //   const { i18n } = useTranslation();

  // const fetchInfo = async () => {
  //   try {
  //     setLoading(true);
  //     var res = await accountApi.getMyProfile();
  //     if (res.statusCode === 200 && res.data) {
  //       setAccount(res.data);
  //     } else {
  //       setAccount(null);
  //     }
  //   } finally {
  //     setLoading(false);
  //   }
  // };

  //   useEffect(() => {
  //     if (account) {
  //       i18n.changeLanguage(account?.language || "en");
  //     }
  //   }, [account]);

  //   const changeLanguage = (language: string) => {
  //     i18n.changeLanguage(language);
  //   };

  const changeAvatar = (avatarUrl: string) => {
    queryClient.setQueryData<AccountData>(["myProfile"], (old) => {
      if (!old) return old;
      return { ...old, avatarUrl };
    });
  };

  return (
    <AuthContext.Provider
      value={{
        account,
        isLoading: isProfileLoading || isLogoutLoading,
        logout: async () => fetchLogout({}),
        fetchInfo: async () => fetchProfile(),
        changeAvatar,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
};

export default AuthContext;

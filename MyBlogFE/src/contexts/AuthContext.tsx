import { accountApi, authApi } from "@/api";
import { useApiMutation, useApiQuery } from "@/hooks";
import type { AccountData } from "@/types/account.type";
import { i18n } from "@/utils";
import { useQueryClient } from "@tanstack/react-query";
import { createContext, useEffect, type ReactNode } from "react";

interface AuthContextReturn {
  account: AccountData | null | undefined;
  isLoading: boolean;
  logout: () => Promise<void>;
  fetchInfo: () => Promise<void>;
  // changeLanguage: (language: string) => void;
  changeAvatar: (avatarUrl: string) => void;
  updateProfile: (account: AccountData) => void;
}

export const AuthContext = createContext<AuthContextReturn | undefined>(
  undefined,
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

  const fetchInfo = async () => {
    await fetchProfile();
  };

  // const changeLanguage = (language: string) => {};

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

  useEffect(() => {
    if (account) {
      i18n.changeLanguage(account?.language || "en");
    }
  }, [account?.language]);

  //   const changeLanguage = (language: string) => {
  //     i18n.changeLanguage(language);
  //   };

  const changeAvatar = (avatarUrl: string) => {
    queryClient.setQueryData<AccountData>(["myProfile"], (old) => {
      if (!old) return old;
      return { ...old, avatarUrl };
    });
  };

  const updateProfile = (account: AccountData) => {
    queryClient.setQueryData<AccountData>(["myProfile"], (old) => {
      if (!old) return old;
      return { ...old, ...account };
    });
  };

  return (
    <AuthContext.Provider
      value={{
        account,
        isLoading: isProfileLoading || isLogoutLoading,
        logout: async () => fetchLogout({}),
        fetchInfo,
        changeAvatar,
        updateProfile,
        // changeLanguage,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
};

export default AuthContext;

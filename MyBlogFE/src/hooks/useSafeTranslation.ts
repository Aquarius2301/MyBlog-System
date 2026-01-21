import type { I18nKey } from "@/utils";
import { useTranslation } from "react-i18next";

export default function useSafeTranslation() {
  const { t: translate, i18n } = useTranslation<I18nKey>();

  const t = (key: I18nKey) => {
    return translate(key);
  };

  const tUnsafe = (key: string) => {
    return translate(key);
  };

  return { t, tUnsafe, i18n };
}

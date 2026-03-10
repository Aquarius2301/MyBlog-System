import i18n from "i18next";
import { initReactI18next } from "react-i18next";
import LanguageDetector from "i18next-browser-languagedetector";

import en from "@/locales/en.json";
import vi from "@/locales/vi.json";
import ja from "@/locales/ja.json";

declare const chrome: any;

i18n
  .use(LanguageDetector)
  .use(initReactI18next)
  .init({
    backend: {
      // Sử dụng chrome.runtime.getURL để lấy đường dẫn tuyệt đối trong extension
      loadPath: chrome.runtime.getURL("locales/{{lng}}.json"),
    },
    resources: {
      en: { translation: en },
      vi: { translation: vi },
      ja: { translation: ja },
    },
    fallbackLng: "en",
    interpolation: {
      escapeValue: false,
    },
  });

export type I18nKey = keyof typeof en;
export default i18n;

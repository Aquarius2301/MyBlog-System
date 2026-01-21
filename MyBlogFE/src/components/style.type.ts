export const FONT_SIZE = {
  xsmall: "10px",
  small: "12px",
  medium: "16px",
  large: "20px",
  xlarge: "24px",
  xxlarge: "32px",
} as const;

export const SPACING = {
  xsmall: "4px",
  small: "8px",
  medium: "16px",
  large: "24px",
  xlarge: "32px",
  xxlarge: "40px",
} as const;

export const BackgroundColor = { light: "#f0f2f5", dark: "#1f1f1f" };
export const TextColor = { light: "#000000", dark: "#ffffff" };
export const BackgroundCardColor = "#ffffff";

export type FontSize = keyof typeof FONT_SIZE;
export type Spacing = keyof typeof SPACING;

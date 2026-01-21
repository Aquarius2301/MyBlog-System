import { useTheme } from "@/hooks";
import type React from "react";
import { FONT_SIZE, TextColor, type FontSize } from "../style.type";

export type TextProps = {
  as?: "p" | "span" | "div";
  color?: string;
  children: React.ReactNode;
  fontSize?: FontSize | number;
  bold?: boolean;
  margin?: string;
  style?: React.CSSProperties;
};

const Text = ({
  as = "span",
  color,
  children,
  fontSize = "medium",
  bold = false,
  margin = "0",
  style,
}: TextProps) => {
  const Component = as;
  const { themeMode } = useTheme();

  return (
    <Component
      style={{
        color: color || TextColor[themeMode],
        fontSize: typeof fontSize === "string" ? FONT_SIZE[fontSize] : fontSize,
        fontWeight: bold ? "bold" : "normal",
        margin,
        ...style,
      }}
    >
      {children}
    </Component>
  );
};

export default Text;

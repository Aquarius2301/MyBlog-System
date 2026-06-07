import { useSafeTranslation } from "@/hooks";
import { Typography } from "antd";
import { useState } from "react";

export type ParagraphProps = {
  children?: string;
  isExpandable?: boolean;
  style?: React.CSSProperties;
};

const Paragraph = ({
  children,
  isExpandable = false,
  style,
}: ParagraphProps) => {
  const [expanded, setExpanded] = useState<boolean>(false);
  const { t } = useSafeTranslation();

  return (
    <Typography.Paragraph
      style={style}
      ellipsis={{
        rows: 1,
        expandable: isExpandable ? "collapsible" : false,
        expanded,
        onExpand: (_, info) => setExpanded(info.expanded),
        symbol: expanded ? t("ShowLess") : t("ShowMore"),
      }}
    >
      {children}
    </Typography.Paragraph>
  );
};

export default Paragraph;

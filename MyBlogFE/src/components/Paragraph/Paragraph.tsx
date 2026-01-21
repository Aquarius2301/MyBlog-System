import { useSafeTranslation } from "@/hooks";
import { Typography } from "antd";
import { useState } from "react";

export type ParagraphProps = {
  content: string;
  isExpandable?: boolean;
};

const Paragraph = ({ content, isExpandable = false }: ParagraphProps) => {
  const [expanded, setExpanded] = useState<boolean>(false);
  const { t } = useSafeTranslation();

  return (
    <Typography.Paragraph
      ellipsis={{
        rows: 1,
        expandable: isExpandable ? "collapsible" : false,
        expanded,
        onExpand: (_, info) => setExpanded(info.expanded),
        symbol: expanded ? t("ShowLess") : t("ShowMore"),
      }}
    >
      {content}
    </Typography.Paragraph>
  );
};

export default Paragraph;

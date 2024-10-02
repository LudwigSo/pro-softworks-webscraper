"use client";

import {
  Bar,
  BarChart,
  CartesianGrid,
  LabelList,
  XAxis,
  YAxis,
} from "recharts";

import {
  ChartConfig,
  ChartContainer,
  ChartTooltip,
  ChartTooltipContent,
} from "@/components/ui/chart";
import { ProjectDto } from "@/api";
import { useEffect, useState } from "react";

export const description = "A bar chart with a custom label";

const chartConfig = {
  number: {
    label: "Name",
    color: "hsl(var(--chart-1))",
  },
  name: {
    label: "Count",
    color: "hsl(var(--chart-2))",
  },
  label: {
    color: "hsl(var(--background))",
  },
} satisfies ChartConfig;

interface ProjectsChartContainerProps {
  projects: ProjectDto[];
}

export function ProjectsChartContainer({
  projects,
}: ProjectsChartContainerProps) {
  const [tagsData, setTagsData] = useState<{ name: string; count: number }[]>(
    []
  );

  useEffect(() => {
    console.log(projects);
    if (!projects || projects.length === 0) {
      return;
    }
    setTagsData(() => {
      const allTags = projects
        .map((project) => project.tags?.map((tag) => tag.name))
        .flat();
      const counts: { [key: string]: number } = {};

      for (const tag of allTags) {
        if (!tag) {
          continue;
        }
        counts[tag] = counts[tag] ? counts[tag] + 1 : 1;
      }

      return Object.keys(counts).map((key) => ({
        name: key,
        count: counts[key],
      }));
    });
  }, [projects]);

  return (
    <div className="p-4 w-full">
      <ChartContainer config={chartConfig} className="min-w-10 w-auto">
        <BarChart
          accessibilityLayer
          data={tagsData}
          layout="vertical"
          margin={{
            right: 16,
          }}
        >
          <CartesianGrid horizontal={false} />
          <YAxis
            dataKey="number"
            type="category"
            tickLine={false}
            tickMargin={10}
            axisLine={false}
            tickFormatter={(value) => value.slice(0, 3)}
            hide
          />
          <XAxis dataKey="number" type="number" hide />
          <ChartTooltip
            cursor={false}
            content={<ChartTooltipContent indicator="line" />}
          />
          <Bar
            dataKey="number"
            layout="vertical"
            fill="var(--color-desktop)"
            radius={4}
          >
            <LabelList
              dataKey="number"
              position="insideLeft"
              offset={8}
              className="fill-[--color-label]"
              fontSize={12}
            />
            <LabelList
              dataKey="number"
              position="right"
              offset={8}
              className="fill-foreground"
              fontSize={12}
            />
          </Bar>
        </BarChart>
      </ChartContainer>
    </div>
  );
}

export default ProjectsChartContainer;

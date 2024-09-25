import { ProjectDto } from "@/api";
import { projectApi } from "@/api-configs";
import { errorToast } from "@/supplements/toasts";
import { useContext, useEffect, useState } from "react";
import { Context } from "@/App";
import dayjs from "dayjs";

import {
  ResizableHandle,
  ResizablePanel,
  ResizablePanelGroup,
} from "@/components/ui/resizable";
import { Input } from "@/components/ui/input";
import _ from "lodash";
import ProjectsDatePicker from "./projects-date-picker";
import ProjectsSorting from "./projects-sorting";
import ProjectsCard from "./projects-card";
import ProjectsChartContainer from "./projects-chart-container";

const ProjectsContainer = () => {
  const [search, setSearch] = useState<string>("");
  const [date, setDate] = useState<string>(dayjs().format("YYYY-MM-DD"));

  const [sortBy, setSortBy] = useState<keyof ProjectDto>("firstSeenAt");
  const sortKeys: (keyof ProjectDto)[] = ["firstSeenAt", "title"];
  const [sortIsReversed, setSortIsReversed] = useState<boolean>(true);

  const [data, setData] = useContext(Context);
  const [selectedProject, setSelectedProject] = useState<ProjectDto | null>(
    null
  );

  useEffect(() => {
    async function getAll() {
      try {
        const projects = await projectApi.projectAllWithAnyTagGet(date);
        setData(projects.data);
      } catch (error) {
        errorToast(error);
      }
    }
    getAll();
  }, [date, setData]);

  useEffect(() => {
    setData((v) =>
      sortIsReversed ? _.sortBy(v, sortBy).reverse() : _.sortBy(v, sortBy)
    );
  }, [setData, sortBy, sortIsReversed]);

  return (
    <ResizablePanelGroup className="flex" direction={"horizontal"}>
      <ResizablePanel
        className="h-100 min-h-screen max-h-screen !overflow-auto"
        defaultSize={100}
      >
        <div className="flex flex-wrap p-6 gap-4 mt-14">
          <Input
            value={search}
            onChange={(e) => setSearch(e.target.value)}
            placeholder="Search projects... (supports regex)"
          />
          <ProjectsDatePicker date={date} setDate={setDate} />
          <ProjectsSorting
            sortBy={sortBy}
            setSortBy={setSortBy}
            sortIsReversed={sortIsReversed}
            setSortIsReversed={setSortIsReversed}
            sortKeys={sortKeys}
          />
        </div>
        <div className="flex flex-wrap p-6 gap-4">
          {data.map((project: ProjectDto) => (
            <ProjectsCard
              key={project.id}
              project={project}
              search={search}
              selectedProject={selectedProject}
              setSelectedProject={setSelectedProject}
            />
          ))}
        </div>
      </ResizablePanel>
      <ResizableHandle withHandle />
      <ResizablePanel defaultSize={0} className="flex">
        <ProjectsChartContainer projects={data} />
      </ResizablePanel>
    </ResizablePanelGroup>
  );
};

export default ProjectsContainer;

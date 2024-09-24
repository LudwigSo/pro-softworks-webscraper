import { ProjectDto, TagDto } from "@/api";
import { projectApi } from "@/api-configs";
import { errorToast } from "@/supplements/toasts";
import { useContext, useEffect, useState } from "react";
import { Context } from "@/App";
import dayjs from "dayjs";
import {
  Card,
  CardHeader,
  CardTitle,
  CardDescription,
  CardContent,
} from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import {
  ResizableHandle,
  ResizablePanel,
  ResizablePanelGroup,
} from "@/components/ui/resizable";
import { Input } from "@/components/ui/input";
import { textFilter } from "@/supplements/textfilter";
import _ from "lodash";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { Button } from "../ui/button";
import { Separator } from "@/components/ui/separator";

const ProjectsContainer = () => {
  const [search, setSearch] = useState<string>("");
  const [sortBy, setSortBy] = useState<keyof ProjectDto>("firstSeenAt");
  const sortKeys: (keyof ProjectDto)[] = ["firstSeenAt", "id", "title"];
  const [data, setData] = useContext(Context);
  const [selectedProject, setSelectedProject] = useState<ProjectDto | null>(
    null
  );

  useEffect(() => {
    async function getAll() {
      try {
        const projects = await projectApi.projectAllWithAnyTagGet();
        setData(projects.data);
      } catch (error) {
        errorToast(error);
      }
    }
    getAll();
  }, [setData]);

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
            placeholder="Search projects..."
          />
          <Select
            value={sortBy}
            onValueChange={(v) => {
              setSortBy(v as keyof ProjectDto);
              setData(_.sortBy(data, [v]));
            }}
          >
            <SelectTrigger className="w-[180px]">
              <SelectValue placeholder="sorted" />
            </SelectTrigger>
            <SelectContent>
              {sortKeys?.map((key, index) => (
                <SelectItem value={key} key={index}>
                  {key}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>
        </div>
        <div className="flex flex-wrap p-6 gap-4">
          {data.map((project: ProjectDto) => (
            <Card
              key={project.id}
              className={`flex-grow cursor-pointer border-2 transition duration-200 ease-in-out transform hover:scale-105 ${
                selectedProject?.id === project.id && " border-blue-500"
              } ${textFilter(project, search) ? "" : "hidden"}`}
              onClick={() => setSelectedProject(project)}
            >
              <CardHeader>
                <CardTitle>{project.title}</CardTitle>
                <CardDescription>
                  {project.firstSeenAt
                    ? dayjs(project.firstSeenAt).format("DD.MM.YYYY - HH:mm")
                    : "N/A"}
                </CardDescription>
              </CardHeader>
              <CardContent className="gap-2 flex flex-wrap">
                {project.tags ? (
                  project.tags.map((tag: TagDto) => (
                    <Badge key={tag.id}>{tag.name}</Badge>
                  ))
                ) : (
                  <div>No Tags</div>
                )}
              </CardContent>
            </Card>
          ))}
        </div>
      </ResizablePanel>
      <ResizableHandle withHandle />
      <ResizablePanel defaultSize={0} className="flex">
        {selectedProject ? (
          <div className="w-full p-4 flex flex-col gap-8 m-auto">
            <h2>{selectedProject.title}</h2>
            <ul className="p-2">
              <li className="flex items-center justify-between">
                <span className="text-muted-foreground">First seen at</span>
                <span>
                  {dayjs(selectedProject.firstSeenAt).format(
                    "DD.MM.YYYY-HH:mm"
                  )}
                </span>
              </li>
              <Separator className="m-2" />

              <li className="flex items-center justify-between">
                <span className="text-muted-foreground">Our Id</span>
                <span>{selectedProject.id}</span>
              </li>
            </ul>

            {selectedProject.url && (
              <Button
                onClick={() =>
                  selectedProject.url &&
                  window.open(selectedProject.url, "_blank")
                }
              >
                Visit Page
              </Button>
            )}
          </div>
        ) : (
          <p className="m-auto">Select a project to see details</p>
        )}
      </ResizablePanel>
    </ResizablePanelGroup>
  );
};

export default ProjectsContainer;

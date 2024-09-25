import dayjs from "dayjs";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "../ui/card";
import { Badge } from "../ui/badge";
import { ProjectDto, TagDto } from "@/api";
import { textFilter } from "@/supplements/textfilter";

interface ProjectsCardProps {
  project: ProjectDto;
  search: string;
  selectedProject: ProjectDto | null;
  setSelectedProject: (project: ProjectDto | null) => void;
}

const ProjectsCard = ({
  project,
  search,
  selectedProject,
  setSelectedProject,
}: ProjectsCardProps) => {
  return (
    <Card
      key={project.id}
      className={`relative min-w-80 flex-grow cursor-pointer border-2 transition-all duration-200 ease-in-out transform hover:scale-105 ${
        selectedProject?.id === project.id && " border-blue-500"
      } ${textFilter(project, search) ? "" : "hidden"}`}
      onClick={() => {
        if (selectedProject?.id === project.id) {
          window.open(project.url ?? "", "_blank");
          setSelectedProject(null);
        } else {
          setSelectedProject(project);
        }
      }}
    >
      {selectedProject?.id === project.id && (
        <div>
          <p className="absolute w-full h-full  flex items-center justify-center rounded-md z-10">
            Click again to visit Project
          </p>
          <div className="absolute w-full h-full bg-blue-400 opacity-70 pointer-events-none flex items-center justify-center rounded-md"></div>
        </div>
      )}
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
  );
};

export default ProjectsCard;

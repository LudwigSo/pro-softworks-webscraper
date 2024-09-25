import { ProjectDto } from "@/api";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "../ui/select";
import _ from "lodash";
import { Button } from "../ui/button";
import { ArrowDown01, ArrowUp10 } from "lucide-react";
interface ProjectsSortingProps {
  sortBy: keyof ProjectDto;
  setSortBy: (value: keyof ProjectDto) => void;
  sortKeys: (keyof ProjectDto)[];
  sortIsReversed: boolean;
  setSortIsReversed: (value: boolean) => void;
}

const ProjectsSorting = ({
  sortBy,
  setSortBy,
  sortKeys,
  sortIsReversed,
  setSortIsReversed,
}: ProjectsSortingProps) => {
  return (
    <>
      <Select
        value={sortBy}
        onValueChange={(v) => {
          setSortBy(v as keyof ProjectDto);
        }}
      >
        <SelectTrigger className="w-[180px]">
          <SelectValue placeholder="sorted" />
        </SelectTrigger>
        <SelectContent>
          {sortKeys?.map((key, index) => (
            <SelectItem value={key} key={index}>
              {_.upperFirst(key.replace(/([a-z])([A-Z])/g, "$1 $2"))}
            </SelectItem>
          ))}
        </SelectContent>
      </Select>
      <Button
        onClick={() => {
          setSortIsReversed(!sortIsReversed);
        }}
        variant={"outline"}
        size={"icon"}
      >
        {sortIsReversed ? <ArrowUp10 /> : <ArrowDown01 />}
      </Button>
    </>
  );
};

export default ProjectsSorting;

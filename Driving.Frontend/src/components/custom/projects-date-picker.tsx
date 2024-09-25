import { useState } from "react";
import { Popover, PopoverContent, PopoverTrigger } from "../ui/popover";
import { Button } from "../ui/button";
import { CalendarIcon } from "lucide-react";
import dayjs from "dayjs";

import { Calendar } from "../ui/calendar";

interface ProjectsDatePickerProps {
  date: string;
  setDate: (date: string) => void;
}

const ProjectsDatePicker = ({ date, setDate }: ProjectsDatePickerProps) => {
  const [calendarOpen, setCalendarOpen] = useState(false);

  return (
    <Popover open={calendarOpen} onOpenChange={setCalendarOpen}>
      <PopoverTrigger asChild>
        <Button
          variant={"outline"}
          className={`w-[280px] justify-start text-left font-normal ${!date ? "text-muted-foreground" : ""}`}
        >
          <CalendarIcon className="mr-2 h-4 w-4" />
          {date ? (
            dayjs(date).format("DD.MM.YYYY")
          ) : (
            <span>Set projects since</span>
          )}
        </Button>
      </PopoverTrigger>
      <PopoverContent className="w-auto p-0">
        <Calendar
          mode="single"
          selected={dayjs(date).toDate()}
          onSelect={(date) => {
            setDate(dayjs(date).format("YYYY-MM-DD"));
            setCalendarOpen(false);
          }}
          initialFocus
        />
      </PopoverContent>
    </Popover>
  );
};

export default ProjectsDatePicker;

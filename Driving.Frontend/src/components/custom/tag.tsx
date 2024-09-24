import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { Badge } from "../ui/badge";
import { TagDto } from "@/api";
import { errorToast, successToast } from "@/supplements/toasts";
import { keyWordApi, tagApi } from "@/api-configs";
import { useState } from "react";
import { Edit, Trash } from "lucide-react";
import { Cross2Icon } from "@radix-ui/react-icons";
import { Separator } from "../ui/separator";
import _ from "lodash";

export function Tag(tag: TagDto & { getAll: () => void }) {
  const [inputValue, setInputValue] = useState("");

  const [isOpen, setIsOpen] = useState(false);

  async function removeTag(id?: number) {
    if (!id) return;
    try {
      await tagApi.tagDeleteIdDelete(id);
      successToast("Tag removed successfully");
      setIsOpen(false);
      tag.getAll();
    } catch (error) {
      errorToast(error);
    }
  }

  async function addKeyword(keyword?: string) {
    if (!keyword) return;
    try {
      await keyWordApi.keywordCreatePost({
        tagId: tag.id,
        value: keyword,
      });
      await tag.getAll();
      successToast("Keyword added successfully");
    } catch (error) {
      errorToast(error);
    }
  }

  async function removeKeyword(id?: number) {
    if (!id) return;
    try {
      await keyWordApi.keywordDeleteIdDelete(id);
      await tag.getAll();
      successToast("Keyword deleted");
    } catch (error) {
      errorToast(error);
    }
  }

  return (
    <Dialog open={isOpen} onOpenChange={setIsOpen} aria-describedby={undefined}>
      <DialogTrigger>
        <Badge className="group relative cursor-pointer m-1 hover:mr-0 transition-all duration-300">
          {tag.name}
          <Edit className="h-3 w-3 opacity-0 group-hover:opacity-100 transition-all duration-300 group-hover:ml-1" />
        </Badge>
      </DialogTrigger>
      <DialogContent className="sm:max-w-[425px]">
        <DialogHeader>
          <DialogTitle>Edit Tag '{tag.name}'</DialogTitle>
        </DialogHeader>
        <DialogDescription className="hidden"></DialogDescription>
        <div className="grid gap-4 py-4">
          <div className="flex gap-4">
            <Input
              id="tag"
              value={inputValue}
              className="col-span-3"
              placeholder="Enter a keyword..."
              onInput={(e) =>
                setInputValue((e.target as HTMLInputElement).value)
              }
            />
            <Button onClick={() => addKeyword(inputValue)}>Add Keyword</Button>
          </div>
          <Separator />
          <div className="flex flex-wrap">
            {_.sortBy(tag.keywords, "value").map((keyword) => (
              <Badge
                key={keyword.id}
                variant="secondary"
                className="group relative m-1 hover:mr-0 transition-all duration-200"
              >
                {keyword.value}
                <Button
                  onClick={() => removeKeyword(keyword.id)}
                  variant="destructive"
                  className="h-4 w-4 opacity-0 group-hover:opacity-100 transition-all duration-200 group-hover:ml-1"
                  size="icon"
                >
                  <Cross2Icon className="h-3 w-3" />
                </Button>
              </Badge>
            ))}
          </div>
          <Separator />
          <div>
            <Button onClick={() => removeTag(tag.id)} variant="destructive">
              <Trash className="mr-2 h-4 w-4" />
              Delete Tag
            </Button>
          </div>
        </div>
      </DialogContent>
    </Dialog>
  );
}

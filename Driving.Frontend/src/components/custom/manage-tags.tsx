import { tagApi } from "@/api-configs";
import { Badge } from "../ui/badge";
import { useState } from "react";
import { Tag } from "@/api";
import { errorToast, successToast } from "@/supplements/toasts";
import { useEffect } from "react";
import { Cross2Icon } from "@radix-ui/react-icons";
import { Button } from "../ui/button";
import { Input } from "../ui/input";
import { Separator } from "@radix-ui/react-dropdown-menu";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";
import {
  Form,
  FormField,
  FormItem,
  FormControl,
  FormDescription,
  FormMessage,
} from "../ui/form";

const ManageTags = () => {
  const formSchema = z.object({
    name: z.string().min(1, {
      message: "Tag must be at least 1 characters.",
    }),
  });

  const form = useForm<z.infer<typeof formSchema>>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      name: "",
    },
  });

  async function addTag(values: z.infer<typeof formSchema>) {
    try {
      await tagApi.tagCreatePost(values);
      await getAll();
      successToast(`Tag: ${values.name} added successfully`);
    } catch (error) {
      errorToast(error);
    }
  }

  const [data, setData] = useState<Tag[] | null>(null);
  async function getAll() {
    try {
      const tags = await tagApi.tagAllGet();
      setData(tags.data);
    } catch (error) {
      errorToast(error);
    }
  }

  async function removeTag(id?: number) {
    if (!id) return;
    try {
      await tagApi.idDelete(id);
      await getAll();
      successToast("Tag removed successfully");
    } catch (error) {
      errorToast(error);
    }
  }

  useEffect(() => {
    getAll();
  }, []);

  return (
    <>
      <p className="pl-6 pt-6">Add Tags</p>
      <Form {...form}>
        <form
          className="p-6 flex gap-2"
          noValidate
          onSubmit={form.handleSubmit(addTag)}
        >
          <FormField
            control={form.control}
            name="name"
            render={({ field }) => (
              <FormItem className="flex-1">
                <FormControl>
                  <Input placeholder="Insert tag name..." {...field} />
                </FormControl>
                <FormDescription></FormDescription>
                <FormMessage />
              </FormItem>
            )}
          />
          <Button type="submit">Submit</Button>
        </form>
      </Form>

      <Separator className="mt-6 w-100 h-[1px] bg-border" />
      <p className="pl-6 pt-6">All Tags</p>
      <div className="flex p-8">
        {data ? (
          <div className="flex flex-wrap gap-2">
            {data.map((tag) => (
              <Badge key={tag.id}>
                {tag.name}
                <Button
                  className="ml-4 h-4 w-4"
                  onClick={() => removeTag(tag.id)}
                  variant="destructive"
                  size="icon"
                >
                  <Cross2Icon className="h-3 w-3" />
                </Button>
              </Badge>
            ))}
          </div>
        ) : (
          <p>No Tags</p>
        )}
      </div>
    </>
  );
};

export default ManageTags;

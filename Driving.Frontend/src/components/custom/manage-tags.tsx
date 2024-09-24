import { tagApi } from "@/api-configs";
import { useState } from "react";
import { errorToast, successToast } from "@/supplements/toasts";
import { useEffect } from "react";
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
import { TagDto } from "@/api";
import { Tag } from "./tag";

const ManageTags = () => {
  const formSchema = z.object({
    name: z.string().min(1, {
      message: "Tag must be at least 1 characters.",
    }),
  });

  //const setProjects = useContext(Context)[1];
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
      //await refreshProjects();
      form.reset();
      successToast(`Tag: ${values.name} added successfully`);
    } catch (error) {
      errorToast(error);
    }
  }

  const [data, setData] = useState<TagDto[] | null>(null);

  async function getAll() {
    try {
      const tags = await tagApi.tagAllGet();
      setData(tags.data);
    } catch (error) {
      errorToast(error);
    }
  }

  // retag request
  // async function refreshProjects() {
  //   try {
  //     await projectApi.projectRetagPost();
  //     const projects = await projectApi.projectAllWithAnyTagGet();

  //     setProjects(projects.data);
  //   } catch (error) {
  //     errorToast(error);
  //   }
  // }

  useEffect(() => {
    getAll();
  }, []);

  return (
    <>
      <p className="pl-6 pt-6">Add Tags</p>
      <Form {...form}>
        <form className="p-6 flex gap-2" onSubmit={form.handleSubmit(addTag)}>
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
          <Button type="submit">Add</Button>
        </form>
      </Form>

      <Separator className="mt-6 w-100 h-[1px] bg-border" />
      <p className="pl-6 pt-6">All Tags</p>
      <div className="flex p-8">
        {data ? (
          <div className="flex flex-wrap">
            {data.map((tag) => (
              <Tag key={tag.id} {...tag} getAll={getAll} />
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

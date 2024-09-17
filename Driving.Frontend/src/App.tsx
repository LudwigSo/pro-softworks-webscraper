import { useEffect, useState } from "react";
import "./index.css";
import { Button } from "./components/ui/button";
import { tagApi } from "./api-configs";
import { Tag } from "./api";
import { Input } from "./components/ui/input";
import {
  ResizableHandle,
  ResizablePanel,
  ResizablePanelGroup,
} from "@/components/ui/resizable";
import { ThemeProvider } from "./components/custom/theme-provider";
import { ModeToggle } from "./components/custom/mode-toggle";
import ProjectHeader from "./components/custom/project-header";
import { Separator } from "./components/ui/separator";
import { Badge } from "./components/ui/badge";

function App() {
  const [data, setData] = useState<Tag[] | null>(null);
  const [loading, setLoading] = useState(true);
  const [inputValue, setInputValue] = useState("");

  async function fetchData() {
    setLoading(true);
    setData(null);
    try {
      const tags = await tagApi.tagAllGet();
      setData(tags.data);
    } catch (error) {
      alert(error);
    } finally {
      setLoading(false);
    }
  }
  async function addTag() {
    try {
      await tagApi.tagCreatePost({ name: inputValue });
      fetchData();
    } catch (error) {
      alert(error);
    }
  }

  useEffect(() => {
    fetchData();
  }, []);

  return (
    <>
      <ThemeProvider defaultTheme="dark" storageKey="vite-ui-theme">
        <ModeToggle />
        <div>
          <ResizablePanelGroup
            direction="horizontal"
            className="h-100 max-h-screen"
          >
            <ResizablePanel defaultSize={20}>
              <ResizablePanelGroup direction="vertical">
                <ResizablePanel defaultSize={25}>
                  <ProjectHeader />
                  <Separator />
                  <p className="pl-6 pt-6">Add Tags</p>
                  <div className="flex items-center justify-center p-4 margin auto">
                    {inputValue}
                    <Input
                      value={inputValue}
                      onInput={(e: React.ChangeEvent<HTMLInputElement>) =>
                        setInputValue(e.target.value)
                      }
                    />
                    <Button onClick={addTag}>Add Tag</Button>
                  </div>
                </ResizablePanel>
                <ResizableHandle withHandle />
                <ResizablePanel defaultSize={25}>
                  <p className="pl-6 pt-6">All Tags</p>
                  <div className="flex p-8">
                    {data ? (
                      <div className="flex flex-wrap gap-2">
                        {data.map((tag) => (
                          <Badge key={tag.id}>{tag.name}</Badge>
                        ))}
                      </div>
                    ) : loading ? (
                      "Loading..."
                    ) : (
                      "Fetch Data"
                    )}
                  </div>
                </ResizablePanel>
              </ResizablePanelGroup>
            </ResizablePanel>
            <ResizableHandle withHandle />
            <ResizablePanel defaultSize={80} className="!overflow-auto">
              <div onClick={() => fetchData()}>
                {data ? (
                  <pre>{JSON.stringify(data, null, 2)}</pre>
                ) : loading ? (
                  "Loading..."
                ) : (
                  "Fetch Data"
                )}
              </div>
            </ResizablePanel>
          </ResizablePanelGroup>
        </div>
      </ThemeProvider>
    </>
  );
}

export default App;

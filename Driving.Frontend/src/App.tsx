import { useEffect, useState } from "react";
import "./index.css";
import { tagApi } from "./api-configs";
import { Tag } from "./api";
import {
  ResizableHandle,
  ResizablePanel,
  ResizablePanelGroup,
} from "@/components/ui/resizable";
import { ThemeProvider } from "./components/custom/theme-provider";
import { ModeToggle } from "./components/custom/mode-toggle";
import ProjectHeader from "./components/custom/project-header";
import { Separator } from "./components/ui/separator";
import { Toaster } from "./components/ui/toaster";
import { errorToast } from "./supplements/toasts";
import ManageTags from "./components/custom/manage-tags";

function App() {
  const [data, setData] = useState<Tag[] | null>(null);
  const [loading, setLoading] = useState(true);

  async function fetchData() {
    setLoading(true);
    setData(null);
    try {
      const tags = await tagApi.tagAllGet();
      setData(tags.data);
    } catch (error) {
      errorToast(error);
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    fetchData();
  }, []);

  return (
    <>
      <Toaster />
      <ThemeProvider defaultTheme="dark" storageKey="vite-ui-theme">
        <ModeToggle />
        <div>
          <ResizablePanelGroup
            direction="horizontal"
            className="h-100 min-h-screen max-h-screen"
          >
            <ResizablePanel defaultSize={20}>
              <ProjectHeader />
              <Separator />
              <ManageTags />
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

import "./index.css";
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
import ManageTags from "./components/custom/manage-tags";
import ProjectsContainer from "./components/custom/projects-container";
import React, { useState } from "react";
import { Project } from "./api";

type ProjectState = [
  Project[],
  React.Dispatch<React.SetStateAction<Project[]>>,
];

export const Context = React.createContext<ProjectState>([[], () => {}]);

function App() {
  const [projects, setProjects]: ProjectState = useState<Project[]>([]);
  return (
    <>
      <Context.Provider value={[projects, setProjects]}>
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
              <ResizablePanel defaultSize={80}>
                <ProjectsContainer />
              </ResizablePanel>
            </ResizablePanelGroup>
          </div>
        </ThemeProvider>
      </Context.Provider>
    </>
  );
}

export default App;

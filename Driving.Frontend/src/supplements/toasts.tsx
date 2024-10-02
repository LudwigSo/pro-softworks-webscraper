import { toast } from "@/hooks/use-toast";

export function errorToast(error: unknown) {
  toast({
    variant: "destructive",
    title: "Uh oh! Something went wrong.",
    description: error instanceof Error ? error.message : String(error),
  });
}

export function successToast(message: string) {
  toast({
    variant: "default",
    title: message,
  });
}

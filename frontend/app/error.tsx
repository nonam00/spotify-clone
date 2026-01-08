"use client";

import { useEffect } from "react";
import { Box } from "@/shared/ui";

type ErrorProps = {
  error: Error & { digest?: string };
  reset: () => void;
}

const Error = ({ error, reset }: ErrorProps) => {
  useEffect(() => {
    console.error("Error caught by error boundary:", error);
  }, [error]);

  return (
    <Box className="h-full flex items-center justify-center">
      <div className="text-center">
        <div className="text-neutral-400 mb-4">Something went wrong.</div>
        <button
          onClick={reset}
          className="px-6 py-2 bg-green-500 hover:bg-green-600 text-black rounded-full transition"
        >
          Try again
        </button>
      </div>
    </Box>
  );
}

export default Error;
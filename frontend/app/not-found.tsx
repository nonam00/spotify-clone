import Link from "next/link";
import { Box } from "@/shared/ui";

const NotFound = () => {
  return (
    <Box className="h-full flex items-center justify-center">
      <div className="text-center">
        <div className="text-neutral-400 text-6xl font-bold mb-4">404</div>
        <div className="text-neutral-400 mb-6">Page not found</div>
        <Link
          href="/"
          className="px-6 py-2 bg-green-500 hover:bg-green-600 text-black rounded-full transition"
        >
          Go to home
        </Link>
      </div>
    </Box>
  );
}

export default NotFound;

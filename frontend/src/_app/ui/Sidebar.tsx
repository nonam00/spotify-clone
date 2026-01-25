"use client";

import { usePathname } from "next/navigation";
import { useMemo } from "react";
import { useShallow } from "zustand/shallow";
import { BiSearch } from "react-icons/bi";
import { HiHome } from "react-icons/hi";
import { twMerge } from "tailwind-merge";

import { Box } from "@/shared/ui";
import type { Playlist } from "@/entities/playlist";
import { usePlayerStore } from "@/widgets/player";
import { Library } from "@/widgets/library";
import { SidebarItem } from "./SidebarItem";

export function Sidebar({
  children,
  playlists,
}: {
  children: React.ReactNode;
  playlists: Playlist[];
}) {
  const pathName = usePathname();
  const playerActiveSongId = usePlayerStore(useShallow((s) => s.activeId));

  const routes = useMemo(
    () => [
      {
        icon: HiHome,
        label: "Home",
        active: pathName === "/",
        href: "/",
      },
      {
        icon: BiSearch,
        label: "Search",
        active: pathName === "/search",
        href: "/search?searchString=&type=all",
      },
    ],
    [pathName]
  );

  return (
    <div
      className={twMerge(
        `flex h-full`,
        playerActiveSongId && "h-[calc(100%-80px)]"
      )}
    >
      <div className="hidden md:flex flex-col gap-y-2 bg-black h-full w-[300px] p-2">
        <Box>
          <div className="flex flex-col gap-y-4 px-5 py-4">
            {routes.map((item) => (
              <SidebarItem key={item.label} {...item} />
            ))}
          </div>
        </Box>
        <Box className="overflow-y-hidden hover:overflow-y-auto h-full">
          <Library playlists={playlists} />
        </Box>
      </div>
      <main className="h-full flex-1 overflow-auto py-2">{children}</main>
    </div>
  );
}
"use client";

import { usePathname } from "next/navigation";
import { useMemo } from "react";
import { BiSearch } from "react-icons/bi";
import { HiHome } from "react-icons/hi";
import { twMerge } from "tailwind-merge";

import { Playlist } from "@/types/types";
import usePlayer from "@/hooks/usePlayer";

import Box from "./Box";
import SidebarItem from "./SidebarItem";
import Library from "./Library";

const Sidebar = ({
   children,
   playlists
}: {
  children: React.ReactNode,
  playlists: Playlist[]
}) => {
  const pathName = usePathname();
  const playerActiveSongId = usePlayer(s => s.activeId);

  const routes = useMemo(() => [
    {
      icon: HiHome,
      label: 'Home',
      active: pathName !== '/search',
      href: '/'
    },
    {
      icon: BiSearch,
      label: 'Search',
      active: pathName === '/search',
      href: '/search?searchString=&type=all'
    }
  ], [pathName]);

  return (
  <div className={twMerge(`flex h-full`, playerActiveSongId && "h-[calc(100%-80px)]")}>
    <div className="hidden md:flex flex-col gap-y-2 bg-black h-full w-[300px] p-2">
      <Box>
        <div className="flex flex-col gap-y-4 px-5 py-4">
          {routes.map((item) => (
            <SidebarItem
              key={item.label}
              {...item}
            />
          ))}
        </div>
      </Box>
      <Box className="overflow-y-hidden hover:overflow-y-auto h-full">
        <Library playlists={playlists} />
      </Box>
    </div>
    <main className="h-full flex-1 overflow-auto py-2">
      {children}
    </main>
  </div>
  );
}

export default Sidebar;

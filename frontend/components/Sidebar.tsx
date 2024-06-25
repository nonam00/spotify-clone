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

interface SidebarProps {
  children: React.ReactNode;
  playlists: Playlist[]
}

const Sidebar: React.FC<SidebarProps> = ({
   children,
   playlists
}) => {
  const pathName = usePathname();
  const player = usePlayer();

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
      href: '/search'
    }
  ], [pathName]);

  return (
  <div className={twMerge(`
      flex
      h-full
    `,
      player.activeId && "h-[calc(100%-80px)]"
  )}>
    <div className="
      hidden
      md:flex
      flex-col
      gap-y-2
      bg-black
      h-full
      w-[300px]
      p-2
    "
    >
      <Box>
        <div 
          className="
            flex
            flex-col
            gap-y-4
            px-5
            py-4
          "
        >
          {routes.map((item) => (
            <SidebarItem
              key={item.label}
              {...item}
            />
          ))}
        </div>
      </Box>
      <Box className="
        overflow-y-hidden
        hover:overflow-y-auto
        h-full
      ">
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

"use client";

import { useRouter } from "next/navigation";
import Image from "next/image";
import {memo} from "react";
import { AiOutlinePlus } from "react-icons/ai";
import { TbPlaylist } from "react-icons/tb";

import { Playlist } from "@/types/types";

import useAuthModal from "@/hooks/useAuthModal";
import useCreateModal from "@/hooks/useCreateModal";
import { useUser } from "@/hooks/useUser";

import MediaItem from "./MediaItem";

const Library = memo(function Library({
  playlists
}: {
  playlists: Playlist[]
}) {
  const openAuthModal = useAuthModal(s => s.onOpen);
  const openCreateModal = useCreateModal(s => s.onOpen);
  const { isAuth } = useUser();
  const router = useRouter();

  const onCreateClick = () => !isAuth ? openAuthModal() : openCreateModal();

  const tryRedirect = (url: string) => {
    if (location.pathname != url) {
      router.push(url);
    }
  }

  return (
    <div className="flex flex-col">
      <div className="flex items-center justify-between px-5 pt-4">
        <div className="inline-flex items-center gap-x-2">
          <TbPlaylist className="text-neutral-400" size={26} />
          <p className="text-neutral-400 font-medium text-md">
            Your Library
          </p>
        </div>
        <button
          onClick={onCreateClick}
          className="text-neutral-400 hover:text-white transition-сolors"
        >
          <AiOutlinePlus size={20}/>
        </button>
      </div>
      <div className="flex flex-col gap-y-5 mt-4 px-3">
        <div
          onClick={() => tryRedirect("/playlist/liked")}
          className="flex items-center p-2 w-full gap-x-3 rounded-md cursor-pointer hover:bg-neutral-800/50"
        >
          <div className="relative rounded-md min-h-[48px] min-w-[48px] overflow-hidden">
            <Image
              fill
              src="/images/liked.png"
              alt="Media Item"
              className="object-cover"
            />
          </div>
          <div className="flex flex-col gap-y-1 overflow-hidden">
            <p className="text-white trancate">Liked Songs</p>
          </div>
        </div>
        {playlists.map((playlist) => (
          <MediaItem
            onClick={(id: string) => tryRedirect(`/playlist/${id}`)}
            key={playlist.id}
            data={playlist}
          />
        ))}
      </div>
    </div>
  );
});

export default Library;

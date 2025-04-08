"use client";

import { useRouter } from "next/navigation";
import {useLayoutEffect} from "react";
import { FaPlay } from "react-icons/fa";
import { AiOutlinePlusCircle } from "react-icons/ai";

import { Song } from "@/types/types";

import MediaItem from "@/components/MediaItem";
import LikeButton from "@/components/LikeButton";
import DeletePlaylistButton from "./DeletePlaylistButton";
import RemoveButton from "./RemoveButton";

import { useUser } from "@/hooks/useUser";
import useOnPlay from "@/hooks/useOnPlay";

const PlaylistContent = ({
  id,
  songs
}: {
  id: string;
  songs: Song[];
}) => {
  const router = useRouter();
  const { isLoading, isAuth } = useUser();
  const onPlay = useOnPlay(songs);

  useLayoutEffect(() => {
    if (!isLoading && !isAuth) {
      router.replace("/")
    }
  }, [isLoading, isAuth, router]);

  const onPlayClick = () => {
    if (songs.length === 0) {
      return;
    }
    onPlay(songs[0].id);
  }
  return (
    <div>
      <div className="flex flex-row align-middle gap-y-2 items-center justify-start px-6 py-3">
        <button
          onClick={onPlayClick}
          className="
            transition
            rounded-full
            flex
            items-center
            justify-start
            bg-green-500
            p-5
            drop-shadow-md
            right-5
            hover:scale-110
          "
        >
          <FaPlay className="text-black" size="20" />
        </button>
        <div className="flex-grow" />
        <button
          onClick={() => { router.push(`/playlist/${id}/add?searchString=&type=all`) }}
          className="flex flex-end mx-5 rounded-full hover:scale-105"
        >
          <AiOutlinePlusCircle className="text-neutral-400" size="35" />
        </button>
        <DeletePlaylistButton playlistId={id} />
      </div>
      <div className="h-0.5 bg-neutral-800/40 w-full" />
      <div className="flex flex-col align-middle gap-y-5 w-full p-6">
        {songs.length === 0 ?
          <div className="flex flex-col text-neutral-400 items-center md:items-start">
            There are no songs in this playlist.
          </div>
          : songs.map((song) => (
            <div
              key={song.id}
              className="flex items-center gap-4 w-full"
            >
              <div className="flex-1">
                <MediaItem
                  onClick={(id: string) => onPlay(id)}
                  data={song}
                />
              </div>
              <div className="flex flex-row gap-x-6">
                <RemoveButton playlistId={id} songId={song.id} />
                <LikeButton songId={song.id} />
              </div>
            </div>
          ))}
      </div>
    </div>
  );
}

export default PlaylistContent;

"use client";

import {useTransition} from "react";
import toast from "react-hot-toast";
import { AiOutlineCloseCircle } from "react-icons/ai";

import {removeSongFromPlaylist} from "@/services/playlists";

const RemoveButton = ({
  playlistId,
  songId,
  callback
}: {
  playlistId: string;
  songId: string;
  callback?: (songId: string) => void;
}) => {
  const [isPending, startTransition] = useTransition();

  const handleRemove = async () => {
    startTransition(async () => {
      const response = await removeSongFromPlaylist(playlistId, songId);
      if (response.ok) {
        if (callback) {
          callback(songId);
        }
        toast.success("The song removed from the playlist");
      } else {
        toast.error("An error occurred while removing the song from the playlist");
      }
    })
  }

  return (
    <button
      onClick={handleRemove}
      disabled={isPending}
      className="hover:opacity-75 cursor-pointer transition"
    >
      <AiOutlineCloseCircle
        className="text-neutral-400 hover:text-red-500"
        size={25}
      />
    </button>
  )
}

export default RemoveButton;

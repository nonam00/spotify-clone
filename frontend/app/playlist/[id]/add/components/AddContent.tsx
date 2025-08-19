"use client";

import { useRouter } from "next/navigation";
import {useTransition} from "react";
import toast from "react-hot-toast";

import { Song } from "@/types/types";
import SongListItem from "@/components/SongListItem";
import {addSongToPlaylist} from "@/services/playlists";

const AddContent= ({
  playlistId,
  songs
}: {
  playlistId: string;
  songs: Song[];
}) => {
  const router = useRouter();
  const [isPending, startTransition] = useTransition();

  if (songs.length === 0) {
    return (
      <div className="flex flex-col gap-y-2 w-full px-6 text-neutral-400">
        No songs found.
      </div>
    )
  }

  const onAddClick = async (songId: string) => {
    if (isPending) return;
    startTransition(async () => {
      const response = await addSongToPlaylist(playlistId, songId);
      if (response.ok) {
        router.refresh();
        toast.success("Song added to playlist")
      } else {
        toast.error("Failed");
      }
    })
  }

  return (
    <div className="flex flex-col gap-y-2 w-full px-6">
      {songs.map((song) => (
        <SongListItem
          key={song.id}
          song={song}
          onClickCallback={onAddClick}
        />
      ))}
    </div>
  );
}

export default AddContent;

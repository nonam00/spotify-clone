"use client";

import { useRouter } from "next/navigation";
import { useCallback, useState, useTransition } from "react";
import toast from "react-hot-toast";

import { Button } from "@/shared/ui";
import { type Song, SongListItem } from "@/entities/song";
import { addSongsToPlaylist } from "@/entities/playlist";

const AddSongsToPlaylistContent = ({
  playlistId,
  songs,
}: {
  playlistId: string;
  songs: Song[];
}) => {
  const router = useRouter();
  const [isPending, startTransition] = useTransition();
  const [toAddList, setToAddList] = useState<string[]>([]);

  const onClearClick = useCallback(() => {
    if (isPending) return;
    startTransition(() => {
      setToAddList([]);
    });
  }, [isPending]);

  const onAddClick = useCallback((songId: string) => {
    if (isPending) return;
    startTransition(() => {
      if (toAddList.includes(songId)) {
        setToAddList((prevSongs) => prevSongs.filter((id) => id !== songId));
      } else {
        setToAddList([...toAddList, songId]);
      }
    });
  }, [isPending, toAddList]);

  const onSaveClick = useCallback(() => {
    if (isPending) return;
    startTransition(async () => {
      const ok = await addSongsToPlaylist(playlistId, toAddList);
      if (ok) {
        setToAddList([]);
        router.refresh();
        toast.success("Songs successfully added to the playlist.");
      } else {
        toast.error("Failed to add like-button to the playlist.");
      }
    });
  }, [isPending, playlistId, router, toAddList]);

  if (songs.length === 0) {
    return (
      <div className="flex flex-col gap-y-2 w-full px-6 text-neutral-400 outline-none">
        No songs found.
      </div>
    );
  }

  return (
    <div className="flex flex-col gap-y-2 w-full px-6 outline-none">
      <div className="flex flex-row gap-x-2">
        <Button
          className="bg-white px-6 py-2 mb-4 w-[25%]"
          type="button"
          onClick={onSaveClick}
          disabled={isPending}
        >
          Save
        </Button>
        <Button
          className="bg-white px-6 py-2 mb-4 w-[25%]"
          type="button"
          onClick={onClearClick}
          disabled={isPending}
        >
          Clear
        </Button>
      </div>
      {songs.map((song) => (
        <SongListItem
          key={song.id}
          song={song}
          onClickCallback={onAddClick}
          selected={toAddList.includes(song.id)}
        />
      ))}
    </div>
  );
};

export default AddSongsToPlaylistContent;
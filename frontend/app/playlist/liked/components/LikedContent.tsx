"use client";

import {memo} from "react";

import { Song } from "@/types/types";
import SongListItem from "@/components/SongListItem";
import useOnPlay from "@/hooks/useOnPlay";

const LikedContent = memo(function LikedContent({
  songs
}: {
  songs: Song[]
}) {
  const onPlay = useOnPlay(songs);

  if (songs.length === 0) {
    return (
      <div className="flex flex-col gap-y-2 px-6 text-neutral-400">
        No liked songs.
      </div>
    )
  }

  return (
    <div className="flex flex-col gap-y-2 w-full p-6">
      {songs.map((song) => (
        <SongListItem
          key={song.id}
          song={song}
          likeButton={true}
          likeDefault={true}
          onClickCallback={onPlay}
        />
      ))}
    </div>
  );
});
 
export default LikedContent;

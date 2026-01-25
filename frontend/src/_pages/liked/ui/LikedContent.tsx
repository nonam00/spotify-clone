"use client";

import { memo } from "react";
import { type Song, SongListItem } from "@/entities/song";
import { LikeButton } from "@/features/like-button";
import { SongOptionsMenu } from "@/features/song-options-menu";
import { useOnPlay } from "@/widgets/player";

const LikedContent = memo(function LikedContent({
  songs
}: {
  songs: Song[]
}) {
  const onPlay = useOnPlay(songs);

  if (songs.length === 0) {
    return (
      <div className="flex flex-col gap-y-2 px-6 text-neutral-400 outline-none">
        No liked songs.
      </div>
    );
  }

  return (
    <div className="flex flex-col gap-y-2 w-full p-6 outline-none">
      {songs.map((song) => (
        <SongListItem
          key={song.id}
          song={song}
          onClickCallback={onPlay}
        >
          <SongOptionsMenu songId={song.id} />
          <LikeButton songId={song.id} defaultValue={true} />
        </SongListItem>
      ))}
    </div>
  );
});

export default LikedContent;
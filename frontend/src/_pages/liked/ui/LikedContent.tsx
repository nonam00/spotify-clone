"use client";

import { memo } from "react";
import {type Song, SongListItem } from "@/entities/song";
import {LikeButton} from "@/features/like-button";
import { useOnPlay } from "@/widgets/player";

const LikedContent = memo(function LikedContent({ songs }: { songs: Song[] }) {
  const onPlay = useOnPlay(songs);

  if (songs.length === 0) {
    return (
      <div className="flex flex-col gap-y-2 px-6 text-neutral-400">
        No liked songs.
      </div>
    );
  }

  return (
    <div className="flex flex-col gap-y-2 w-full p-6">
      {songs.map((song) => (
        <SongListItem
          key={song.id}
          song={song}
          onClickCallback={onPlay}
        >
          <LikeButton songId={song.id} defaultValue={true} />
        </SongListItem>
      ))}
    </div>
  );
});

export default LikedContent;
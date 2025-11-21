"use client";

import type { Song } from "@/entities/song/model";
import { SongListItem } from "@/entities/song/ui";
import { useOnPlay } from "@/features/player";

const SearchContent = ({ songs }: { songs: Song[] }) => {
  const onPlay = useOnPlay(songs);

  if (songs.length === 0) {
    return (
      <div className="flex flex-col gap-y-2 w-full px-6 text-neutral-400">
        No songs found.
      </div>
    );
  }
  return (
    <div className="flex flex-col gap-y-2 w-full px-6">
      {songs.map((song) => (
        <SongListItem
          key={song.id}
          song={song}
          onClickCallback={onPlay}
        />
      ))}
    </div>
  );
};

export default SearchContent;
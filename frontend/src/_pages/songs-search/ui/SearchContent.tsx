"use client";

import { type Song, SongListItem } from "@/entities/song";
import { useOnPlay } from "@/widgets/player";
import {SongOptionsMenu} from "@/features/song-options-menu";

const SearchContent = ({ songs }: { songs: Song[] }) => {
  const onPlay = useOnPlay(songs);

  if (songs.length === 0) {
    return (
      <div className="flex flex-col gap-y-2 w-full px-6 text-neutral-400 outline-none">
        No songs found.
      </div>
    );
  }

  return (
    <div className="flex flex-col gap-y-2 w-full px-6 outline-none">
      {songs.map((song) => (
        <SongListItem
          key={song.id}
          song={song}
          onClickCallback={onPlay}
        >
          <SongOptionsMenu songId={song.id} />
        </SongListItem>
      ))}
    </div>
  );
};

export default SearchContent;
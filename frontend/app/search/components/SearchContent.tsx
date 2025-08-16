"use client";

import { Song } from "@/types/types";
import useOnPlay from "@/hooks/useOnPlay";
import SongListItem from "@/components/SongListItem";

const SearchContent = ({
  songs
}: {
  songs: Song[];
}) => {
  const onPlay = useOnPlay(songs);

  if (songs.length === 0) {
    return (
      <div className="flex flex-col gap-y-2 w-full px-6 text-neutral-400">
        No songs found.
      </div>
    )
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
}
 
export default SearchContent;

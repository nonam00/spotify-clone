"use client";

import { memo } from "react";
import type { Song } from "@/entities/song/model";
import { SongGridItem } from "@/entities/song/ui";
import { useOnPlay } from "@/features/player";

const PageContent = memo(function PageContent({ songs }: { songs: Song[] }) {
  const onPlay = useOnPlay(songs);

  if (songs.length === 0) {
    return <div className="mt-4 text-neutral-400">No songs available.</div>;
  }

  return (
    <div className="
      grid grid-cols-2 sm:grid-cols-3 md:grid-cols-3 lg:grid-cols-4 xl:grid-cols-5 2xl:grid-cols-8 gap-4 mt-4
    ">
      {songs.map((item) => (
        <SongGridItem
          key={item.id}
          onClick={(id: string) => onPlay(id)}
          song={item}
        />
      ))}
    </div>
  );
});

export default PageContent;
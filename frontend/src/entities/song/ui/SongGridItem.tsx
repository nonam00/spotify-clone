"use client";

import Image from "next/image";
import React, { memo } from "react";

import { CLIENT_FILES_URL } from "@/shared/config/api";
import { PlayButton } from "@/shared/ui";
import type { Song } from "../model";

const SongGridItem = memo(function SongItem({
  song,
  onClick,
}: {
  song: Song;
  onClick: (id: string) => void;
}) {
  return (
    <div
      onClick={() => onClick(song.id)}
      className="
        relative group flex flex-col items-center justify-center
        p-3 overflow-hidden gap-x-4
        rounded-md bg-neutral-400/5 hover:bg-neutral-400/10
        cursor-pointer transition
      "
    >
      <div className="relative aspect-square w-full h-full rounded-md overflow-hidden">
        <Image
          className="object-cover"
          src={`${CLIENT_FILES_URL}/download-url?type=image&file_id=${song.imagePath}`}
          fill
          alt={song.title}
          loading="lazy"
          unoptimized
        />
      </div>
      <div className="flex flex-col items-start w-full pt-4 gap-y-1">
        <p className="font-semibold truncate w-full">{song.title}</p>
        <p className="text-neutral-400 text-sm pb-4 w-full truncate">
          By {song.author}
        </p>
      </div>
      <div className="absolute bottom-24 right-5">
        <PlayButton />
      </div>
    </div>
  );
});

export default SongGridItem;
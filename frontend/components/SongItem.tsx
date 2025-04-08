"use client";

import Image from "next/image";

import { Song } from "@/types/types";
import PlayButton from "@/components/PlayButton";
import { CLIENT_API_URL } from "@/api/http";
import React, {memo} from "react";

const SongItem = memo(function SongItem({
  data,
  onClick
}: {
  data: Song;
  onClick: (id: string) => void
}) {
  return (
    <div
      onClick={() => onClick(data.id)}
      className="
        relative
        group
        flex flex-col
        items-center
        justify-center
        rounded-md
        overflow-hidden
        gap-x-4
        bg-neutral-400/5
        hover:bg-neutral-400/10
        cursor-pointer
        transition
        p-3
      "
    >
      <div className="relative aspect-square w-full h-full rounded-md overflow-hidden">
        <Image 
          className="object-cover"
          src={`${CLIENT_API_URL}/files/image/${data.imagePath}`}
          fill
          alt="Image"
          unoptimized
        />
      </div>
      <div className="flex flex-col items-start w-full pt-4 gap-y-1">
        <p className="font-semibold truncate w-full">
          {data.title}
        </p>
        <p className="text-neutral-400 text-sm pb-4 w-full truncate">
          By {data.author}
        </p>
      </div>
      <div className="absolute bottom-24 right-5">
        <PlayButton /> 
      </div>
    </div>
  );
});
 
export default SongItem;

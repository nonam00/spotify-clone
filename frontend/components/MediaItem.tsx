"use client";

import Image from "next/image";

import useLoadImage from "@/hooks/useLoadImage";
import { Playlist, Song } from "@/types/types";

interface MediaItemProps {
  data: Song | Playlist;
  onClick?: (id: string) => void;
}

const MediaItem: React.FC<MediaItemProps> = ({
  data,
  onClick
}) => {
  const imageUrl = useLoadImage(data);

  const handleClick = () => {
    if (onClick) {
      return onClick(data.id);
    }
    // default turn on player
  }
  return (
    <div
      onClick={handleClick}
      className="
        flex
        items-center
        gap-x-3
        cursor-pointer
        hover:bg-neutral-800/50
        w-full
        p-2
        rounded-md  
      "
    >
      <div
        className="
          relative
          rounded-md
          min-h-[48px]
          min-w-[48px]
          overflow-hidden
        "
      >
        <Image
          fill
          src={imageUrl || '/images/playlist.webp'}
          alt="Media Item"
          className="object-cover"
        />
      </div>
      <div className="
        flex
        flex-col
        gap-y-1
        overflow-hidden
      ">
        <p className="text-white trancate">
          {data.title}
        </p>
        <p className="text-neutral-400 text-sm truncate">
          {(data as Song)?.author}
        </p>
      </div>
    </div>
  );
}
 
export default MediaItem;

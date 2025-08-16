import {Playlist, Song} from "@/types/types";
import Image from "next/image";
import {CLIENT_API_URL} from "@/helpers/api";

// component for user playlists in sidebar library or songs list in home page
const MediaItem = ({
  data,
}: {
  data: Song | Playlist;
}) => {
  return (
    <div className="flex items-center w-full p-2 rounded-md gap-x-3 cursor-pointer hover:bg-neutral-800/50">
      <div className="relative rounded-md min-h-[48px] min-w-[48px] overflow-hidden">
        <Image
          fill
          src={data.imagePath ? `${CLIENT_API_URL}/files/image/${data.imagePath}` : '/images/playlist.webp'}
          alt={data.title}
          unoptimized
          loading="lazy"
          className="object-cover"
        />
      </div>
      <div className="flex flex-col gap-y-1 overflow-hidden">
        <p className="text-white trancate">
          {data.title}
        </p>
        <p className="text-neutral-400 text-sm truncate">
          {(data as Song)?.author}
        </p>
      </div>
    </div>
  );
};

export default MediaItem;
import { CLIENT_FILES_URL } from "@/shared/config/api";
import type { Song } from "../model";

const SongItem = ({
  song,
  selected
}: {
  song: Song;
  selected?: boolean;
}) => {
  return (
    <div className={`
      flex items-center w-full p-2 rounded-lg gap-x-3
      cursor-pointer transition-all duration-200
      hover:bg-neutral-800/30 ${selected ? "bg-neutral-800/40" : ""}
    `}>
      <div className="relative rounded-lg h-[56px] w-[56px] flex-shrink-0 overflow-hidden shadow-md ring-2 ring-neutral-700/30">
        <img
          src={song.imagePath ?
            `${CLIENT_FILES_URL}/download-url?type=image&file_id=${song.imagePath}`
            : '/images/playlist.webp'}
          alt={song.title}
          className="object-cover w-full h-full"
        />
      </div>
      <div className="flex flex-col gap-y-1 overflow-hidden flex-1">
        <p className="text-white font-medium truncate">
          {song.title}
        </p>
        <p className="text-neutral-400 text-sm truncate">
          {song.author}
        </p>
      </div>
    </div>
  );
};

export default SongItem;
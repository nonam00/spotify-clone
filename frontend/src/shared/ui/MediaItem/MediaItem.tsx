import Image from "next/image";
import { CLIENT_FILES_URL } from "@/shared/config/api";

type MediaItemProps = {
  title: string;
  imagePath?: string | null;
  author?: string;
  selected?: boolean;
}

export function MediaItem ({
  title,
  imagePath,
  author,
  selected,
}: MediaItemProps) {
  return (
    <div
      className={`
      flex items-center w-full p-2 rounded-md gap-x-3
      cursor-pointer hover:bg-neutral-800/50 ${selected ? "bg-neutral-800/50" : ""}
    `}
    >
      <div className="relative rounded-md min-h-[48px] min-w-[48px] overflow-hidden">
        <Image
          fill
          src={
            imagePath
              ? `${CLIENT_FILES_URL}/download-url?type=image&file_id=${imagePath}`
              : "/images/playlist.webp"
          }
          alt={title}
          unoptimized
          loading="lazy"
          className="object-cover"
        />
      </div>
      <div className="flex flex-col gap-y-1 overflow-hidden">
        <p className="text-white truncate">{title}</p>
        {author && (<p className="text-neutral-400 text-sm truncate">{author}</p>)}
      </div>
    </div>
  );
}
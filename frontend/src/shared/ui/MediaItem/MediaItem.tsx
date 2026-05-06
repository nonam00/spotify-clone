import Image from "next/image";
import { CLIENT_FILES_URL } from "@/shared/config/api";
import { ExplicitBadge } from "@/shared/ui";

type MediaItemProps = {
  title: string;
  imagePath?: string | null;
  author?: string;
  isExplicit?: boolean;
  selected?: boolean;
}

export function MediaItem ({
  title,
  imagePath,
  author,
  isExplicit,
  selected,
}: MediaItemProps) {
  return (
    <div
      className={`
      flex items-center w-full p-2 rounded-md gap-x-3
      cursor-pointer hover:bg-neutral-800/50 ${selected ? "bg-neutral-800/50" : ""}
    `}
      title={author ? `${title} — ${author}` : title}
      aria-label={author ? `${title} — ${author}` : title}
    >
      <div className="relative rounded-md h-12 w-12 overflow-hidden shrink-0">
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
      <div className="flex flex-col gap-y-1 overflow-hidden w-full">
        <p className="text-white truncate w-full">{title}</p>
        {(author || isExplicit) && (
          <div className="flex items-center gap-x-1.5 w-full min-w-0">
            {isExplicit && <ExplicitBadge />}
            {author && (<p className="text-neutral-400 text-sm truncate">{author}</p>)}
          </div>
        )}
      </div>
    </div>
  );
}
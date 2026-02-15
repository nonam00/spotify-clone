import { MediaItem } from "@/shared/ui";
import type { Song } from "../model";

const SongListItem = ({
  song,
  onClickCallback,
  selected,
  children,
}: {
  song: Song;
  onClickCallback?: (id: string) => void;
  selected?: boolean;
  children?: React.ReactNode;
}) => {
  function onClick() {
    if (onClickCallback) {
      onClickCallback(song.id);
    }
  }
  return (
    <div key={song.id} className="flex items-center gap-x-4 w-full">
      <div onClick={onClick} className="flex-1 min-w-0">
        <MediaItem
          title={song.title}
          author={song.author}
          imagePath={song.imagePath}
          selected={selected}
        />
      </div>
      <div className="flex flex-row gap-x-6">
        {children}
      </div>
    </div>
  );
};

export default SongListItem;
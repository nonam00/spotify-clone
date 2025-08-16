import {Song} from "@/types/types";
import LikeButton from "@/components/LikeButton";
import MediaItem from "@/components/ui/MediaItem";

const SongListItem = ({
  song,
  onClickCallback,
  likeButton,
  likeDefault,
  children
}: {
  song: Song
  onClickCallback: (id: string) => void,
  likeButton?: boolean,
  likeDefault?: boolean
  children?: React.ReactNode
}) => {
  return (
    <div
      key={song.id}
      className="flex items-center gap-x-4 w-full"
    >
      <div
        onClick={() => onClickCallback(song.id)}
        className="flex-1"
      >
        <MediaItem data={song}/>
      </div>
      <div className="flex flex-row gap-x-6">
        {children}
        {likeButton ? <LikeButton songId={song.id} defaultValue={likeDefault}/> : null}
      </div>
    </div>
  );
}

export default SongListItem;
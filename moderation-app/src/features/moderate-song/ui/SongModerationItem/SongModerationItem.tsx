import type { Song } from "@/entities/song";
import { MediaItem, Button } from "@/shared/ui";
import AudioPlayer from "../AudioPlayer/AudioPlayer.tsx";
import { useModerationStore } from "../../model/store";
import { useConfirmModalStore } from "@/shared/store/confirmModalStore";

type SongModerationItemProps = {
  song: Song;
}

const SongModerationItem = ({ song }: SongModerationItemProps) => {
  const { publishSong, deleteSong, selectedSongs, toggleSongSelection, isLoading } = useModerationStore();
  const { onOpen } = useConfirmModalStore();
  const isSelected = selectedSongs.includes(song.id);

  const handlePublish = () => {
    onOpen(
      "Publish this song?",
      `Are you sure you want to publish song "${song.title}" by ${song.author}? After publish it will be available to all users.`,
      async () => {
        await publishSong(song.id);
      }
    );
  };

  const handleDelete = () => {
    onOpen(
      "Delete this song?",
      `Are you sure you want to delete song "${song.title}" by ${song.author}? This action cannot be undone.`,
      async () => {
        await deleteSong(song.id);
      }
    );
  };

  return (
    <div className="flex flex-col gap-y-4 w-full p-5 rounded-xl bg-neutral-800/50 border border-neutral-700/30 hover:border-neutral-600/50 hover:shadow-lg transition-all duration-300">
      <div className="flex items-center gap-x-4 w-full">
        <input
          type="checkbox"
          checked={isSelected}
          onChange={() => toggleSongSelection(song.id)}
          disabled={isLoading}
          className="w-5 h-5 cursor-pointer disabled:opacity-50 disabled:cursor-not-allowed accent-green-500"
        />
        <div className="flex-1">
          <MediaItem data={song} />
        </div>
        <div className="flex gap-x-3">
          <Button
            onClick={handlePublish}
            disabled={isLoading}
            className="bg-green-500 hover:bg-green-600 text-white px-5 py-2.5 text-sm font-medium shadow-md"
          >
            Publish
          </Button>
          <Button
            onClick={handleDelete}
            disabled={isLoading}
            className="bg-red-500 hover:bg-red-600 text-white px-5 py-2.5 text-sm font-medium shadow-md"
          >
            Delete
          </Button>
        </div>
      </div>
      <div className="pl-9 border-t border-neutral-700/30 pt-4">
        <AudioPlayer song={song} />
      </div>
    </div>
  );
};

export default SongModerationItem;


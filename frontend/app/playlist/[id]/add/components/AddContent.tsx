"use client";

import { useRouter } from "next/navigation";
import toast from "react-hot-toast";

import MediaItem from "@/components/MediaItem";
import { Song } from "@/types/types";
import addSongToPlaylist from "@/services/playlists/addSongToPlaylist";

interface AddContentProps {
  playlistId: string,
  songs: Song[];
}

const AddContent: React.FC<AddContentProps> = ({
  playlistId,
  songs
}) => {
  const router = useRouter();

  if (songs.length === 0) {
    return (
      <div
        className="
          flex
          flex-col
          gap-y-2
          w-full
          px-6
          text-neutral-400
        "
      >
        No songs found.
      </div>
    )
  }

  const onAddClick = async (songId: string) => {
    const response = await addSongToPlaylist(playlistId, songId);

    if (response.ok) {
      router.refresh();
      toast.success("Song added to playlist")
    } else {
      toast.error("Failed");
    }
  }

  return (
    <div className="flex flex-col gap-y-2 w-full px-6">
      {songs.map((song) => (
        <div
          key={song.id}
          className="flex items-center gap-x-4 w-full"
        >
          <div className="flex-1">
            <MediaItem
              onClick={(id: string) => { onAddClick(id) }}
              data={song}
            />
          </div>
        </div>
      ))}
    </div>
  );
}

export default AddContent;

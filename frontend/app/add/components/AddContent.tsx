"use client";

import { useRouter } from "next/navigation";

import $api from "@/api/http";
import MediaItem from "@/components/MediaItem";
import { Song } from "@/types/types";
import toast from "react-hot-toast";
import { AxiosError } from "axios";

interface SearchContentProps {
  playlistId: string,
  songs: Song[];
}

const AddContent: React.FC<SearchContentProps> = ({
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

  const onAddClick = (songId: string) => {
    $api.post(`/playlists/${playlistId}/songs/${songId}`)
      .then(() => {
        router.refresh();
        toast.success("Song added to playlist")
      })
      .catch((error: AxiosError) => {
        console.log(error.response?.data);
        toast.error("Failed");
      });
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
              onClick={(id: string) => {onAddClick(id)}}
              data={song}
            />
          </div>
        </div>
      ))}
    </div>
  );
}
 
export default AddContent;

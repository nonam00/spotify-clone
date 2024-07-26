"use client";

import { useRouter } from "next/navigation";
import toast from "react-hot-toast";
import { AiOutlineCloseCircle } from "react-icons/ai";

import removeSongFromPlaylist from "@/services/playlists/removeSongFromPlaylist";

interface RemoveButtonProps {
  playlistId: string
  songId: string
}

const RemoveButton: React.FC<RemoveButtonProps> = ({
  playlistId,
  songId
}) => {
  const router = useRouter();

  const handleRemove = async () => {
    const response = await removeSongFromPlaylist(playlistId, songId);

    if (response.ok) {
      router.refresh();
      toast.success("The song removed from the playlist");
    } else {
      toast.error("An error occurred while removing the song from the playlist");
    }
  }

  return (
    <button
      onClick={handleRemove}
      className="
        hover:opacity-75
        transition
      "
    >
      <AiOutlineCloseCircle
        className="
          text-neutral-400
          hover:text-red-500
        "
        size={25}
      />
    </button>
  )
}

export default RemoveButton;

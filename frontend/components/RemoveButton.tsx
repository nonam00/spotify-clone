"use client";

import { useRouter } from "next/navigation";
import { AxiosError } from "axios";
import toast from "react-hot-toast";
import { AiOutlineCloseCircle } from "react-icons/ai"; 

import $api from "@/api/http";

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
    await $api.delete(`/playlists/${playlistId}/songs/${songId}`)
      .then(() => {
        router.refresh();
        toast.success("The song removed from the playlist");
      })
      .catch((error: AxiosError) => {
        toast.error("An error occurred while removing the song from the playlist");
        console.log(error.response?.data);
      });
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

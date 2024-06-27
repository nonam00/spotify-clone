"use client";

import { useRouter } from "next/navigation";
import { AxiosError } from "axios";
import toast from "react-hot-toast";
import { FaTrash } from "react-icons/fa";

import $api from "@/api/http";
import useConfirmModal from "@/hooks/useConfirmModal";

interface DeletePlaylistButtonProps {
  playlistId: string
}

const DeletePlaylistButton: React.FC<DeletePlaylistButtonProps> = ({
  playlistId
}) => {
  const router = useRouter();
  const { onOpen, setAction, setDescription } = useConfirmModal();
  
  const deletePlaylist = async () => {
    await $api.delete(`/playlists/${playlistId}`)
      .then(() => {
        router.push("/");
        toast.success("The playlist was succesfully deleted")
      })
      .catch((error: AxiosError) => {
        toast.error("An error occurred while deleting the playlist");
        console.log(error.response?.data);
      });
  }

  const onClick = () => {
    setDescription("This action will delete this playlist from your library");
    setAction(deletePlaylist);
    onOpen();
  }

  return (
    <button
      onClick={onClick}
      className="flex flex-end hover:opacity-75"
    >
      <FaTrash className="text-neutral-400 hover:text-red-500" size="24"/>
    </button>
  )
}

export default DeletePlaylistButton;

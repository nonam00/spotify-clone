"use client";

import { useRouter } from "next/navigation";
import toast from "react-hot-toast";
import { FaTrash } from "react-icons/fa";
import { useShallow } from "zustand/shallow";

import useConfirmModal from "@/hooks/useConfirmModal";
import {deletePlaylist} from "@/services/playlists";

const DeletePlaylistButton = ({
  playlistId
}: {
  playlistId: string
}) => {
  const router = useRouter();
  const [onOpen, setAction, setDescription] = useConfirmModal(useShallow(s => [
    s.onOpen,
    s.setAction,
    s.setDescription
  ]));

  const handleDeletePlaylist = async () => {
    const response = await deletePlaylist(playlistId);
    if (response.ok) {
      router.push("/");
      toast.success("The playlist was successfully deleted")
    } else {
      toast.error("An error occurred while deleting the playlist");
    }
  }

  const onClick = () => {
    setDescription("This action will delete this playlist from your library");
    setAction(handleDeletePlaylist);
    onOpen();
  }

  return (
    <button
      onClick={onClick}
      className="flex flex-end hover:opacity-75"
    >
      <FaTrash className="text-neutral-400 hover:text-red-500" size="24" />
    </button>
  )
}

export default DeletePlaylistButton;

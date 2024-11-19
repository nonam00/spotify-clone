"use client";

import { useRouter } from "next/navigation";
import { useEffect } from "react";

import { useUser } from "@/hooks/useUser";
import useAuthModal from "@/hooks/useAuthModal";
import useCreateModal from "@/hooks/useCreateModal";
import useUploadModal from "@/hooks/useUploadModal";

import createPlaylist from "@/services/playlists/createPlaylist";

import Button from "./Button";
import Modal from "./Modal";
import toast from "react-hot-toast";

const CreateModal = () => {
  const router = useRouter();
  const { isAuth } = useUser();
  const [onClose, isOpen] = useCreateModal(s => [s.onClose, s.isOpen]);
  const openAuthModal = useAuthModal(s => s.onOpen);
  const openUploadModal = useUploadModal(s => s.onOpen);
  
  useEffect(() => {
    if(isAuth) {
      router.refresh();
      onClose();
    }
  }, [isAuth, onClose, router]);

  const onChange = (open: boolean) => {
    if(!open) {
      onClose();
    }
  }

  const onPlaylistClick = async () => {
    if (!isAuth) {
      onClose();
      return openAuthModal();
    }

    const response = await createPlaylist();
    
    if (response.ok) {
      const id = await response.text();
      onClose();
      router.push(`/playlist/${id}`);
      router.refresh();
    } else {
      toast("Failed on creating playlist");
    }
  }

  const onUploadClick = () => {
    onClose();
    if (!isAuth) {
      return openAuthModal();
    }
    return openUploadModal();
  }

  return (
    <Modal
      title="What you want to do"
      description=""
      isOpen={isOpen}
      onChange={onChange}
    >
      <div className="
        flex
        flex-col
        items-center
        justify-center
      ">
        <Button
          onClick={onPlaylistClick}
          className="
            my-3
            font-medium
          "
        >
          Create Playlist
        </Button>
        <Button
          onClick={onUploadClick}
          className="
            my-3
            hover:bg-neutral-700
            bg-transparent
            text-neutral-300
            font-medium
          "
        >
          Upload Song
        </Button>
      </div>
    </Modal>
  );
};

export default CreateModal;

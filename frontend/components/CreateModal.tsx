"use client";

import { useRouter } from "next/navigation";
import { useEffect } from "react";

import $api from "@/api/http";

import { useUser } from "@/hooks/useUser";
import useAuthModal from "@/hooks/useAuthModal";
import useCreateModal from "@/hooks/useCreateModal";
import useUploadModal from "@/hooks/useUploadModal";

import Button from "./Button";
import Modal from "./Modal";
import toast from "react-hot-toast";

const CreateModal = () => {
  const router = useRouter();
  const { isAuth } = useUser();
  const { onClose, isOpen } = useCreateModal();
  const authModal = useAuthModal();
  const uploadModal = useUploadModal();
  
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

  const onPlaylistClick = () => {
    if (!isAuth) {
      onClose();
      return authModal.onOpen();
    }
    $api.post("/playlists/")
      .then(response => {
        if (response.status >= 200 && response.status < 400) {
          onClose()
          router.push(`/playlist?id=${response.data}`)
          router.refresh();
        }
      })
      .catch((error) => {
        console.log(error);
        toast("Failed on creating playlist");
      });
  }

  const onUploadClick = () => {
    onClose();
    if (!isAuth) {
      return authModal.onOpen();
    }
    return uploadModal.onOpen();
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

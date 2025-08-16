"use client";

import { useRouter } from "next/navigation";
import { useShallow } from "zustand/shallow";
import toast from "react-hot-toast";

import { useUser } from "@/hooks/useUser";
import useAuthModal from "@/hooks/useAuthModal";
import useCreateModal from "@/hooks/useCreateModal";
import useUploadModal from "@/hooks/useUploadModal";

import createPlaylist from "@/services/playlists/createPlaylist";

import Button from "@/components/ui/Button";
import Modal from "@/components/ui/Modal";

const CreateModal = () => {
  const router = useRouter();
  const { isAuth } = useUser();
  const [onClose, isOpen] = useCreateModal(useShallow(s => [s.onClose, s.isOpen]));
  const openAuthModal = useAuthModal(s => s.onOpen);
  const openUploadModal = useUploadModal(s => s.onOpen);

  const onChange = (open: boolean) => {
    if(!open) {
      onClose();
    }
  }

  const onPlaylistClick = async () => {
    onClose();

    if (!isAuth) {
      return openAuthModal();
    }

    const response = await createPlaylist();

    if (response.ok) {
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
      <div className="flex flex-col items-center justify-center">
        <Button
          onClick={onPlaylistClick}
          className="my-3 font-medium"
        >
          Create Playlist
        </Button>
        <Button
          onClick={onUploadClick}
          className="my-3 bg-transparent text-neutral-300 font-medium hover:bg-neutral-700"
        >
          Upload Song
        </Button>
      </div>
    </Modal>
  );
};

export default CreateModal;

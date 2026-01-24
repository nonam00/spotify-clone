"use client";

import { useRouter } from "next/navigation";
import { memo, useCallback } from "react";
import { useShallow } from "zustand/shallow";
import toast from "react-hot-toast";

import { Button, Modal } from "@/shared/ui";
import { createPlaylist } from "@/entities/playlist";
import { useCreateModalStore, useUploadModalStore } from "../model";

const CreateModal = memo(function CreateModal() {
  const router = useRouter();

  const { onClose, isOpen } = useCreateModalStore(
    useShallow((s) => ({
      onClose: s.onClose,
      isOpen: s.isOpen,
    })),
  );

  const openUploadModal = useUploadModalStore(useShallow((s) => s.onOpen));

  const onChange = useCallback((open: boolean) => {
    if (!open) {
      onClose();
    }
  }, [onClose]);

  const onPlaylistClick = useCallback(async () => {
    onClose();
    const success = await createPlaylist();
    if (success) {
      router.refresh();
    } else {
      toast.error("Failed on creating playlist");
    }
  }, [onClose, router]);

  const onUploadClick = useCallback(() => {
    onClose();
    return openUploadModal();
  }, [onClose, openUploadModal]);

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
});

export default CreateModal;
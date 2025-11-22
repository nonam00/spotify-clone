"use client";

import { useState, useEffect } from "react";

import { AuthModal } from "@/features/auth";
import { ConfirmModal } from "@/features/confirm-modal";
import { SongUploadModal, CreateModal } from "@/widgets/library";
import { UpdatePlaylistModal } from "@/widgets/playlist";

const ModalProvider = () => {
  const [isMounted, setIsMounted] = useState(false);

  useEffect(() => {
    setIsMounted(true);
  }, []);

  if (!isMounted) {
    return null;
  }

  return (
    <>
      <AuthModal />
      <CreateModal />
      <SongUploadModal />
      <UpdatePlaylistModal />
      <ConfirmModal />
    </>
  );
};

export default ModalProvider;
"use client";

import { useState, useEffect } from "react";

import AuthModal from "@/components/AuthModal";
import CreateModal from "@/components/CreateModal";
import SongUploadModal from "@/components/SongUploadModal";
import UpdatePlaylistModal from "@/components/UpdatePlaylistModal";
import ConfirmModal from "@/components/ConfirmModal";

const ModalProvider = () => {
  const [isMounted, setIsMounted] = useState(false);

  useEffect(() => {
    setIsMounted(true);
  }, []);

  if(!isMounted) {
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
  )
}

export default ModalProvider;

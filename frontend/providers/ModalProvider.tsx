"use client";

import { useState, useEffect } from "react";

import AuthModal from "@/components/AuthModal";
import CreateModal from "@/components/CreateModal";
import SongUploadModal from "@/components/SongUploadModal";
import PlaylistModal from "@/components/PlaylistModal";
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
      <PlaylistModal />
      <ConfirmModal />
    </>
  )
}

export default ModalProvider;

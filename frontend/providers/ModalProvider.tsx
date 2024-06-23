"use client";

import { useState, useEffect } from "react";

import AuthModal from "@/components/AuthModal";
import CreateModal from "@/components/CreateModal";
import UploadModal from "@/components/UploadModal";
import PlaylistModal from "@/components/PlaylistModal";

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
      <UploadModal />
      <PlaylistModal />
    </>
  )
}

export default ModalProvider;

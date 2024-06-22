"use client";

import { useState, useEffect } from "react";

import AuthModal from "@/components/AuthModal";
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
      <UploadModal />
      <PlaylistModal />
    </>
  )
}

export default ModalProvider;

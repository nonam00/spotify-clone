"use client";

import { useCallback } from "react";
import { useShallow } from "zustand/shallow";

import type { Song } from "@/entities/song";
import { useAuthStore, useAuthModalStore } from "@/features/auth";
import { usePlayerStore } from "../model";

export function useOnPlay(songs: Song[]) {
  const { setActiveId, setIds } = usePlayerStore();
  const openAuthModal = useAuthModalStore(useShallow((s) => s.onOpen));
  const { isAuthenticated } = useAuthStore();

  return useCallback(
    (id: string) => {
      if (!isAuthenticated) {
        usePlayerStore.persist.clearStorage();
        return openAuthModal();
      }

      setActiveId(id);
      setIds(songs.map((song) => song.id));
    },
    [openAuthModal, isAuthenticated, setActiveId, setIds, songs]
  );
}
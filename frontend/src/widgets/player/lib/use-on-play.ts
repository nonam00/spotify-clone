"use client";

import { useCallback } from "react";

import type { Song } from "@/entities/song";
import { useAuthStore, useAuthModalStore } from "@/features/auth";
import { usePlayerStore } from "../model";

export function useOnPlay(songs: Song[]) {
  const { setActiveId, setIds } = usePlayerStore();
  const openAuthModal = useAuthModalStore(s => s.onOpen);
  const isAuthenticated = useAuthStore(s => s.isAuthenticated);

  return useCallback(
    (id: string) => {
      if (!isAuthenticated) {
        return openAuthModal();
      }

      setActiveId(id);
      setIds(songs.map((song) => song.id));
    },
    [isAuthenticated, setActiveId, setIds, songs, openAuthModal]
  );
}
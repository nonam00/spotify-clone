"use client";

import { useCallback } from "react";

import type { Song } from "@/entities/song";
import { useAuthStore, useAuthModalStore } from "@/features/auth";
import { usePlayerStore } from "../model";
import {useShallow} from "zustand/shallow";

export function useOnPlay(songs: Song[]) {
  const { activeId, setActiveId, setIds } = usePlayerStore(useShallow((s) => ({
    activeId: s.activeId,
    setActiveId: s.setActiveId,
    setIds: s.setIds,
  })));
  const openAuthModal = useAuthModalStore(s => s.onOpen);
  const isAuthenticated = useAuthStore(s => s.isAuthenticated);

  return useCallback(
    (id: string) => {
      if (!isAuthenticated) return openAuthModal();
      if (id === activeId) return;
      setActiveId(id);
      setIds(songs.map((song) => song.id));
    },
    [isAuthenticated, openAuthModal, activeId, setActiveId, setIds, songs]
  );
}
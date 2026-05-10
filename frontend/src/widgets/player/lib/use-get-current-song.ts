"use client";

import { useEffect, useRef } from "react";
import { useShallow } from "zustand/shallow";
import { Song } from "@/entities/song";
import { usePlayerStore } from "../model";

// Get song by active id from track list store
export function useGetCurrentSong(): {
  currentSong: Song | undefined;
} {
  const { activeId, currentSong, fetchCurrentSong } = usePlayerStore(
    useShallow((s) => ({
      activeId: s.activeId,
      currentSong: s.currentSong,
      fetchCurrentSong: s.fetchCurrentSong,
    }))
  );

  const abortControllerRef = useRef<AbortController | null>(null);

  // Fetch song data when activeId changes
  useEffect(() => {
    abortControllerRef.current = new AbortController();

    fetchCurrentSong(abortControllerRef.current.signal).catch(console.error);

    return () => {
      abortControllerRef.current?.abort();
      abortControllerRef.current = null;
    }
  }, [activeId, fetchCurrentSong]);

  return { currentSong };
}
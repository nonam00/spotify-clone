"use client";

import { useLayoutEffect, useState, useTransition } from "react";
import { useShallow } from "zustand/shallow";
import { getSongById, Song } from "@/entities/song";
import { usePlayerStore } from "@/widgets/player/model";

// Get song by active id from track list store
export function useGetCurrentSong(): {
  currentSong: Song | undefined;
  isLoading: boolean;
} {
  const { activeId, cache, setCachedSong, removeSongId } = usePlayerStore(
    useShallow((s) => ({
      activeId: s.activeId,
      cache: s.cache,
      setCachedSong: s.setCachedSong,
      removeSongId: s.removeId,
    }))
  );

  const [currentSong, setCurrentSong] = useState<Song | undefined>(
    activeId ? cache[activeId] : undefined
  );
  const [isLoading, startTransition] = useTransition();

  // Fetch song data when activeId changes
  useLayoutEffect(() => {
    if (!activeId) {
      setCurrentSong(undefined);
      return;
    }

    // Check cache first
    if (cache[activeId]) {
      setCurrentSong(cache[activeId]);
      return;
    }

    const abortController = new AbortController();

    startTransition(async () => {
      const song = await getSongById(activeId, abortController);
      if (!song) {
        setCurrentSong(undefined);
        removeSongId(activeId);
      } else {
        setCurrentSong(song);
        setCachedSong(song);
      }
    })

    return () => {
      abortController.abort();
    };
  }, [activeId, cache, removeSongId, setCachedSong]);

  return { currentSong, isLoading }
}
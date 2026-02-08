"use client";

import { useEffect, useState, useCallback, useRef } from "react";
import { useShallow } from "zustand/shallow";
import { getSongById, Song } from "@/entities/song";
import { usePlayerStore } from "../model";

// Get song by active id from track list store
export function useGetCurrentSong(): {
  currentSong: Song | undefined;
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
  const abortControllerRef = useRef<AbortController | null>(null);

  const loadCurrentSong = useCallback(async (activeId: string, signal: AbortSignal) => {
    const song = await getSongById(activeId, signal);
    if (!song) {
      setCurrentSong(undefined);
      removeSongId(activeId);
    } else {
      setCurrentSong(song);
      setCachedSong(song);
    }
  }, [removeSongId, setCachedSong])

  // Fetch song data when activeId changes
  useEffect(() => {
    if (!activeId) {
      setCurrentSong(undefined);
      return;
    }

    // Check cache first
    if (cache[activeId]) {
      setCurrentSong(cache[activeId]);
      return;
    }

    abortControllerRef.current = new AbortController();

    loadCurrentSong(activeId, abortControllerRef.current.signal).catch(console.error);

    return () => {
      abortControllerRef.current?.abort();
      abortControllerRef.current = null;
    }
  }, [activeId, cache, loadCurrentSong]);

  return { currentSong };
}
"use client";

import { useState, useLayoutEffect, useMemo } from "react";
import { CLIENT_API_URL, CLIENT_FILES_URL } from "@/shared/config/api";
import type { Song } from "@/entities/song/model";
import { usePlayerStore } from "@/features/player";
import { PlayerContent } from "../ui";

const Player = () => {
  const activeSongId = usePlayerStore((s) => s.activeId);
  const [song, setSong] = useState<Song | undefined>(undefined);
  const [isLoading, setIsLoading] = useState<boolean>(false);

  useLayoutEffect(() => {
    if (!activeSongId) {
      setSong(undefined);
      return;
    }

    const abortController = new AbortController();

    setIsLoading(true);

    fetch(`${CLIENT_API_URL}/songs/${activeSongId}`, {
      signal: abortController.signal,
    })
      .then((res) => res.json())
      .then((data) => {
        setSong(data);
        setIsLoading(false);
      })
      .catch(() => {
        setIsLoading(false);
      });

    return () => {
      abortController.abort();
    };
  }, [activeSongId]);

  const songUrl = useMemo(() => {
    if (!song) return "";
    return `${CLIENT_FILES_URL}/download-url?type=audio&file_id=${song.songPath}`;
  }, [song]);

  if (!song || !activeSongId || isLoading) {
    return null;
  }

  return (
    <div className="fixed bottom-0 bg-black w-full h-[80px]">
      <PlayerContent key={songUrl} song={song} songUrl={songUrl} />
    </div>
  );
};

export default Player;


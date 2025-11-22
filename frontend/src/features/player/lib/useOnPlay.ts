import { useCallback } from "react";
import type { Song } from "@/entities/song/model";
import { useAuthStore, useAuthModalStore } from "@/features/auth/model";
import { usePlayerStore } from "../model";

const useOnPlay = (songs: Song[]) => {
  const { setActiveId, setIds } = usePlayerStore();
  const openAuthModal = useAuthModalStore((s) => s.onOpen);
  const { isAuthenticated } = useAuthStore();

  return useCallback(
    (id: string) => {
      if (!isAuthenticated) {
        return openAuthModal();
      }

      setActiveId(id);
      setIds(songs.map((song) => song.id));
    },
    [openAuthModal, isAuthenticated, setActiveId, setIds, songs]
  );
};

export default useOnPlay;


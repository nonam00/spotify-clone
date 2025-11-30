import { create } from "zustand";

type AudioStore = {
  activeSongId: string | null;
  setActiveSongId: (id: string | null) => void;
}

export const useAudioStore = create<AudioStore>((set) => ({
  activeSongId: null,
  setActiveSongId: (id: string | null) => set({ activeSongId: id }),
}));
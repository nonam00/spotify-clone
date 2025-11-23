import { create } from "zustand";
import { persist } from "zustand/middleware";
import {Song} from "@/entities/song/model";

type SongCache = Record<string, Song>;

type PlayerStore = {
  ids: string[];
  activeId?: string;
  volume: number;
  cache: SongCache;
  setActiveId: (id: string) => void;
  setIds: (ids: string[]) => void;
  setNextId: () => void;
  setPreviousId: () => void;
  setVolume: (value: number) => void;
  setCachedSong: (song: Song) => void;
  reset: () => void;
}

export const usePlayerStore = create<PlayerStore>()(
  persist(
    (set, get) => ({
      ids: [],
      activeId: undefined,
      volume: 1,
      cache: {},
      setActiveId: (id: string) => set({ activeId: id }),
      setIds: (ids: string[]) => set({ ids }),
      setNextId: () => {
        const state = get();
        if (state.ids.length === 0) return;

        const currentIndex = state.ids.findIndex((id) => id === state.activeId);
        const nextId = state.ids[currentIndex + 1];
        set({ activeId: nextId ?? state.ids[0] });
      },
      setPreviousId: () => {
        const state = get();
        if (state.ids.length === 0) return;

        const currentIndex = state.ids.findIndex((id) => id === state.activeId);
        const previousId = state.ids[currentIndex - 1];
        set({ activeId: previousId ?? state.ids[state.ids.length - 1] });
      },
      setVolume: (value: number) => set({ volume: value }),
      setCachedSong: (song: Song) =>
        set((state) => ({
          cache: { ...state.cache, [song.id]: song }
        })),
      reset: () => set({ ids: [], activeId: undefined, volume: 1, cache: {} }),
    }),
    {
      name: "player-storage",
      partialize: (state) => ({
        ids: state.ids,
        activeId: state.activeId,
        volume: state.volume,
      }),
    }
  )
);
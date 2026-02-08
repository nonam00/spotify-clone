import { create } from "zustand";
import { persist } from "zustand/middleware";
import { Song } from "@/entities/song";

type SongCache = Record<string, Song>;

type PlayerStore = {
  ids: string[];
  activeId?: string;
  volume: number;
  cache: SongCache;
  isRehydrated: boolean;
  timestamp: number;
  setActiveId: (id: string) => void;
  setIds: (ids: string[]) => void;
  setNextId: () => void;
  setPreviousId: () => void;
  removeId: (id: string) => void;
  setVolume: (value: number) => void;
  setCachedSong: (song: Song) => void;
  setRehydrated: (value: boolean) => void;
  reset: () => void;
}

const STORAGE_TTL = 1000 * 60 * 60 * 24; // 24 hours

export const usePlayerStore = create<PlayerStore>()(
  persist(
    (set, get) => ({
      ids: [],
      activeId: undefined,
      volume: 1,
      cache: {},
      isRehydrated: false,
      timestamp: Date.now(),
      setActiveId: (id: string) => set({ activeId: id, timestamp: Date.now() }),
      setIds: (ids: string[]) => set({ ids, timestamp: Date.now() }),
      setNextId: () => {
        const state = get();
        if (state.ids.length === 0) return;

        const currentIndex = state.ids.findIndex((id) => id === state.activeId);
        const nextId = state.ids[currentIndex + 1];
        set({ activeId: nextId ?? state.ids[0], timestamp: Date.now() });
      },
      setPreviousId: () => {
        const state = get();
        if (state.ids.length === 0) return;

        const currentIndex = state.ids.findIndex((id) => id === state.activeId);
        const previousId = state.ids[currentIndex - 1];
        set({ activeId: previousId ?? state.ids[state.ids.length - 1], timestamp: Date.now() });
      },
      removeId: (id: string) => {
        const state = get();
        if (id === state.activeId) {
          set({ activeId: undefined });
        }
        set({ ids: state.ids.filter((id) => id !== state.activeId), timestamp: Date.now() });
      },
      setVolume: (value: number) => set({ volume: value, timestamp: Date.now() }),
      setCachedSong: (song: Song) =>
        set((state) => ({
          cache: { ...state.cache, [song.id]: song }
        })),
      setRehydrated: (value: boolean) => set({ isRehydrated: value }),
      reset: () => set({ ids: [], activeId: undefined, volume: 1, cache: {}, timestamp: Date.now() }),
    }),
    {
      name: "player-storage",
      partialize: (state) => ({
        ids: state.ids,
        activeId: state.activeId,
        volume: state.volume,
      }),
      onRehydrateStorage: () => (state) => {
        if (state) {
          if (Date.now() - (state.timestamp ?? 0) > STORAGE_TTL) {
            state.reset();
            usePlayerStore.persist.clearStorage();
          } else {
            state.setRehydrated(true);
          }
        }
      }
    }
  )
);
import { create } from "zustand";
import { persist } from "zustand/middleware";
import { getSongById, Song } from "@/entities/song";
import { LyricsSegment, SongCache } from "./types";
import { getLyricsBySongId } from "../api";

type PlayerStore = {
  ids: string[];
  activeId?: string;
  currentSong: Song | undefined;
  currentLyrics: LyricsSegment[];
  cache: SongCache;
  volume: number;

  isRehydrated: boolean;
  timestamp: number;

  setActiveId: (id: string) => void;
  setIds: (ids: string[]) => void;
  fetchCurrentSong: (signal?: AbortSignal) => Promise<void>;
  fetchCurrentSongLyrics: (signal?: AbortSignal) => Promise<void>;

  setNextId: () => void;
  setPreviousId: () => void;

  setVolume: (value: number) => void;
  setCachedSong: (song: Song) => void;

  setRehydrated: (value: boolean) => void;
  reset: () => void;
}

const STORAGE_TTL = 1000 * 60 * 60 * 24; // 24 hours

export const usePlayerStore = create<PlayerStore>()(
  persist(
    (set, get, store) => ({
      ids: [],
      activeId: undefined,
      cache: {},
      currentSong: undefined,
      currentLyrics: [],
      volume: 1,
      isRehydrated: false,
      timestamp: Date.now(),

      setActiveId: (id: string) => set({
        activeId: id,
        currentSong: undefined,
        currentLyrics: [],
        timestamp: Date.now()
      }),
      setIds: (ids: string[]) => set({ ids, timestamp: Date.now() }),
      fetchCurrentSong: async (signal?: AbortSignal) => {
        const state = get();
        if (!state.activeId) {
          set({currentSong: undefined});
          return;
        }

        // Check cache first
        if (state.cache[state.activeId]) {
          set({currentSong: state.cache[state.activeId]});
          return;
        }

        try {
          const song = await getSongById(state.activeId, signal);
          if (!song) {
            set({currentSong: undefined});
            return;
          }
          set({currentSong: song});
          state.setCachedSong(song);
        } catch (error) {
          console.error(error);
        }
      },
      fetchCurrentSongLyrics: async (signal?: AbortSignal) => {
        const state = get();
        if (!state.activeId) {
          return;
        }
        const lyrics = await getLyricsBySongId(state.activeId, signal);
        if (!lyrics) {
          set({currentLyrics: []});
          return;
        }
        set({currentLyrics: lyrics});
      },
      setNextId: () => {
        const state = get();
        if (state.ids.length === 0) return;

        const currentIndex = state.ids.findIndex((id) => id === state.activeId);
        const nextId = state.ids[currentIndex + 1];
        state.setActiveId(nextId ?? state.ids[0]);
      },
      setPreviousId: () => {
        const state = get();
        if (state.ids.length === 0) return;

        const currentIndex = state.ids.findIndex((id) => id === state.activeId);
        const previousId = state.ids[currentIndex - 1];
        state.setActiveId(previousId ?? state.ids[state.ids.length - 1]);
      },
      setVolume: (value: number) => set({ volume: value, timestamp: Date.now() }),
      setCachedSong: (song: Song) =>
        set((state) => ({
          cache: { ...state.cache, [song.id]: song }
        })),
      setRehydrated: (value: boolean) => set({ isRehydrated: value }),
      reset: () => {
        set({ ids: [], activeId: undefined, volume: 1, cache: {}, timestamp: Date.now() });
        store.persist.clearStorage();
      },
    }),
    {
      name: "player-storage",
      partialize: (state) => ({
        ids: state.ids,
        activeId: state.activeId,
        volume: state.volume,
        timestamp: state.timestamp,
      }),
      onRehydrateStorage: () => (state) => {
        if (!state) return;

        if (Date.now() - (state.timestamp ?? 0) > STORAGE_TTL) {
          state.reset();
          return;
        }

        if (state.ids.length !== 0) {
          state.setRehydrated(true);
          return;
        }
      }
    }
  )
);
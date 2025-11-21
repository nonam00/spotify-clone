import { create } from "zustand";
import {
  type Song,
  getUnpublishedSongs, 
  publishSong as publishSongApi,
  publishSongs as publishSongsApi,
  deleteSong as deleteSongApi, 
  deleteSongs as deleteSongsApi 
} from "@/entities/song";

type SongsStore = {
  songs: Song[];
  selectedSongs: string[];
  isLoading: boolean;
  error: string | null;
  fetchSongs: () => Promise<void>;
  publishSong: (songId: string) => Promise<void>;
  publishSelectedSongs: () => Promise<void>;
  deleteSong: (songId: string) => Promise<void>;
  deleteSelectedSongs: () => Promise<void>;
  toggleSongSelection: (songId: string) => void;
  selectAll: () => void;
  clearSelection: () => void;
}

export const useSongsStore = create<SongsStore>((set, get) => ({
  songs: [],
  selectedSongs: [],
  isLoading: false,
  error: null,

  fetchSongs: async () => {
    set({ isLoading: true, error: null });
    try {
      const data = await getUnpublishedSongs();
      set({ songs: data.songs, isLoading: false });
    } catch (error) {
      set({ 
        error: error instanceof Error ? error.message : "Failed to fetch songs",
        isLoading: false 
      });
    }
  },

  publishSong: async (songId: string) => {
    set({ isLoading: true, error: null });
    try {
      await publishSongApi(songId);
      await get().fetchSongs();
      set({ isLoading: false });
    } catch (error) {
      set({ 
        error: error instanceof Error ? error.message : "Failed to publish song",
        isLoading: false 
      });
    }
  },

  publishSelectedSongs: async () => {
    const { selectedSongs } = get();
    if (selectedSongs.length === 0) return;

    set({ isLoading: true, error: null });
    try {
      await publishSongsApi(selectedSongs);
      await get().fetchSongs();
      set({ selectedSongs: [], isLoading: false });
    } catch (error) {
      set({ 
        error: error instanceof Error ? error.message : "Failed to publish songs",
        isLoading: false 
      });
    }
  },

  deleteSong: async (songId: string) => {
    set({ isLoading: true, error: null });
    try {
      await deleteSongApi(songId);
      await get().fetchSongs();
      set({ isLoading: false });
    } catch (error) {
      set({ 
        error: error instanceof Error ? error.message : "Failed to delete song",
        isLoading: false 
      });
    }
  },

  deleteSelectedSongs: async () => {
    const { selectedSongs } = get();
    if (selectedSongs.length === 0) return;

    set({ isLoading: true, error: null });
    try {
      await deleteSongsApi(selectedSongs);
      await get().fetchSongs();
      set({ selectedSongs: [], isLoading: false });
    } catch (error) {
      set({ 
        error: error instanceof Error ? error.message : "Failed to delete songs",
        isLoading: false 
      });
    }
  },

  toggleSongSelection: (songId: string) => {
    set((state) => {
      const isSelected = state.selectedSongs.includes(songId);
      return {
        selectedSongs: isSelected
          ? state.selectedSongs.filter(id => id !== songId)
          : [...state.selectedSongs, songId]
      };
    });
  },

  selectAll: () => {
    set((state) => ({
      selectedSongs: state.songs.map(song => song.id)
    }));
  },

  clearSelection: () => {
    set({ selectedSongs: [] });
  },
}));
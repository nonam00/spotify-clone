import { create } from "zustand";

import { type ManagedUser } from "@/entities/user";
import {getUserSongs, type Song, unpublishSong} from "@/entities/song";

type UserSongsModalStore = {
  isOpen: boolean;
  user: ManagedUser | null;
  songs: Song[];
  isLoading: boolean;
  unpublishingSongId: string | null;
  error: string | null;
  open: (user: ManagedUser) => void;
  close: () => void;
  unpublishSong: (songId: string) => Promise<void>;
};

function extractError(error: unknown) {
  return error instanceof Error ? error.message : "Failed to load songs";
}

export const useUserSongsModalStore = create<UserSongsModalStore>((set) => ({
  isOpen: false,
  user: null,
  songs: [],
  isLoading: false,
  unpublishingSongId: null,
  error: null,

  open: (user) => {
    set({ isOpen: true, user, songs: [], isLoading: true, error: null });
    void getUserSongs(user.id)
      .then((data) => {
        const songs = data.songs ?? [];
        set((state) => ({
          songs,
          isLoading: false,
          user: state.user ? { ...state.user, uploadedSongsCount: songs.length } : state.user,
        }));
      })
      .catch((error) => {
        set({ error: extractError(error), isLoading: false });
      });
  },

  close: () => set({ isOpen: false, user: null, songs: [], error: null, unpublishingSongId: null }),

  unpublishSong: async (songId) => {
    set({ unpublishingSongId: songId, error: null });
    try {
      await unpublishSong(songId);
      set((state) => {
        const index = state.songs.findIndex((song) => song.id === songId);
        const songToUnpublish = state.songs[index];
        const songs: Song[] = [
          ...state.songs.slice(0, index),
          {
            ...songToUnpublish,
            isPublished: false,
          },
          ...state.songs.slice(index + 1),
        ];
        return {
          songs,
          unpublishingSongId: null,
        };
      });
    } catch (error) {
      set({ error: extractError(error), unpublishingSongId: null });
      throw error;
    }
  },
}));
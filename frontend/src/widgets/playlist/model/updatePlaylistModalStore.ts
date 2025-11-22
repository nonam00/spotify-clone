import { create } from "zustand";
import type { Playlist } from "@/entities/playlist/model";

type UpdatePlaylistModalStore = {
  isOpen: boolean;
  playlist: Playlist | undefined;
  onOpen: () => void;
  onClose: () => void;
  setPlaylist: (playlist: Playlist | undefined) => void;
}

export const useUpdatePlaylistModalStore = create<UpdatePlaylistModalStore>((set) => ({
  isOpen: false,
  onOpen: () => set({ isOpen: true }),
  onClose: () => set({ isOpen: false }),
  playlist: undefined,
  setPlaylist: (playlist) => set({ playlist }),
}));
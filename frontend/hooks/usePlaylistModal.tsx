import { create } from "zustand";
import { ModalStore, Playlist } from "@/types/types";

interface PlaylistModalStore extends ModalStore {
  playlist: Playlist | undefined;
  setPlaylist: (playlist: Playlist | undefined) => void;
}

const usePlaylistModal = create<PlaylistModalStore>((set) => ({
  isOpen: false,
  onOpen: () => set({ isOpen: true }),
  onClose: () => set({ isOpen: false }),
  playlist: undefined,
  setPlaylist: (playlist: Playlist | undefined) => set({
    playlist: playlist
  }),
}));

export default usePlaylistModal;
import { create } from "zustand";
import { ModalStore, Playlist } from "@/types/types";

interface PlaylistModalStore extends ModalStore {
  id: string;
  title: string;
  description: string;
  imagePath: string;
  setPlaylist: (playlist: Playlist) => void;
};

const usePlaylistModal = create<PlaylistModalStore>((set) => ({
  isOpen: false,
  onOpen: () => set({ isOpen: true }),
  onClose: () => set({ isOpen: false }),
  id: "",
  title: "",
  description: "",
  imagePath: "",
  setPlaylist: (playlist: Playlist) => set({
    id: playlist.id,
    title: playlist.title,
    description: playlist.description,
    imagePath: playlist.imagePath,
  }),
}));

export default usePlaylistModal;

import { create } from "zustand";

type UploadModalStore = {
  isOpen: boolean;
  onOpen: () => void;
  onClose: () => void;
}

export const useUploadModalStore = create<UploadModalStore>((set) => ({
  isOpen: false,
  onOpen: () => set({ isOpen: true }),
  onClose: () => set({ isOpen: false }),
}));
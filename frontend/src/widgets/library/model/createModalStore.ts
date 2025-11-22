import { create } from "zustand";

type CreateModalStore = {
  isOpen: boolean;
  onOpen: () => void;
  onClose: () => void;
}

export const useCreateModalStore = create<CreateModalStore>((set) => ({
  isOpen: false,
  onOpen: () => set({ isOpen: true }),
  onClose: () => set({ isOpen: false }),
}));


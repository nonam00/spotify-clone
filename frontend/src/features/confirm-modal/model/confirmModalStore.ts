import { create } from "zustand";

type ConfirmModalStore = {
  isOpen: boolean;
  action: () => Promise<void>;
  description: string;
  onOpen: () => void;
  onClose: () => void;
  setAction: (action: () => Promise<void>) => void;
  setDescription: (description: string) => void;
}

export const useConfirmModalStore = create<ConfirmModalStore>((set) => ({
  action: async () => {},
  description: "",
  isOpen: false,
  onOpen: () => set({ isOpen: true }),
  onClose: () =>
    set({ isOpen: false, action: async () => {}, description: "" }),
  setAction: (action) => set({ action }),
  setDescription: (description) => set({ description }),
}));
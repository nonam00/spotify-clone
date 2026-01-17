import { create } from "zustand";

type ConfirmModalStore = {
  isOpen: boolean;
  title: string;
  description: string;
  onConfirm: (() => Promise<void>) | (() => void) | null;
  onOpen: (title: string, description: string, onConfirm: (() => Promise<void>) | (() => void)) => void;
  onClose: () => void;
}

export const useConfirmModalStore = create<ConfirmModalStore>((set) => ({
  isOpen: false,
  title: "",
  description: "",
  onConfirm: null,
  onOpen: (title, description, onConfirm) => 
    set({ isOpen: true, title, description, onConfirm }),
  onClose: () => set({ isOpen: false, title: "", description: "", onConfirm: null }),
}));
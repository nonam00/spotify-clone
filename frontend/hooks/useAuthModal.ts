import { create } from "zustand";
import { ModalStore } from "@/types/types";

type AuthModalStore = ModalStore;

const useAuthModal = create<AuthModalStore>((set) => ({
  isOpen: false,
  onOpen: () => set({isOpen: true}),
  onClose: () => set({isOpen: false})
}));

export default useAuthModal;

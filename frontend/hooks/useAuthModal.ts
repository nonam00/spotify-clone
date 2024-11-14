import { create } from "zustand";
import { ModalStore } from "@/types/types";

interface AuthModalStore extends ModalStore {}

const useAuthModal = create<AuthModalStore>((set) => ({
  isOpen: false,
  onOpen: () => set({isOpen: true}),
  onClose: () => set({isOpen: false})
}));

export default useAuthModal;

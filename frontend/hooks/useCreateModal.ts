import { ModalStore } from "@/types/types";
import { create } from "zustand";

interface CreateModalStore extends ModalStore {}

const useCreateModal = create<CreateModalStore>((set) => ({
  isOpen: false,
  onOpen: () => set({isOpen: true}),
  onClose: () => set({isOpen: false})
}));

export default useCreateModal;

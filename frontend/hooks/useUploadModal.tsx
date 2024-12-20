import { ModalStore } from "@/types/types";
import { create } from "zustand";

interface UploadModalStore extends ModalStore {}

const useUploadModal = create<UploadModalStore>((set) => ({
  isOpen: false,
  onOpen: () => set({ isOpen: true }),
  onClose: () => set({ isOpen: false })
}));

export default useUploadModal;

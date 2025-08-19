import { ModalStore } from "@/types/types";
import { create } from "zustand";

type UploadModalStore = ModalStore;

const useUploadModal = create<UploadModalStore>((set) => ({
  isOpen: false,
  onOpen: () => set({ isOpen: true }),
  onClose: () => set({ isOpen: false })
}));

export default useUploadModal;

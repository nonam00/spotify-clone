import { create } from "zustand";
import { ModalStore } from "@/types/types";

interface ConfirmModalStore extends ModalStore {
  action: () => Promise<void>
  description: string
  setAction: (action: () => Promise<void>) => void;
  setDescription: (description: string) => void;
};

const useConfirmModal = create<ConfirmModalStore>((set) => ({
  action: async () => {},
  description: "",
  isOpen: false, 
  onOpen: () => set({isOpen: true}),
  onClose: () => set({isOpen: false, action: async () => {}, description: ""}),
  setAction: (action) => set({action: action}),
  setDescription: (description: string) => set({description: description})
}));

export default useConfirmModal;

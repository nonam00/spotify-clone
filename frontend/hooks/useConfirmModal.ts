import { create } from "zustand";

interface ConfirmModalStore {
  action: () => Promise<void>
  description: string
  isOpen: boolean;
  onOpen: () => void;
  onClose: () => void;
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
  setDescription: (description) => set({description: description})
}));

export default useConfirmModal;

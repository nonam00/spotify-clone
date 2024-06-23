import { create } from "zustand";

interface CreateModalStore {
  isOpen: boolean;
  onOpen: () => void;
  onClose: () => void;
};

const useCreateModal = create<CreateModalStore>((set) => ({
  isOpen: false,
  onOpen: () => set({isOpen: true}),
  onClose: () => set({isOpen: false})
}));

export default useCreateModal;

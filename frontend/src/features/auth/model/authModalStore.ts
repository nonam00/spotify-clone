import { create } from "zustand";

type AuthModalStore = {
  isOpen: boolean;
  currentView: 'login' | 'register' | 'forgot-password';
  onOpen: () => void;
  onClose: () => void;
  setView: (view: 'login' | 'register' | 'forgot-password') => void;
}

export const useAuthModalStore = create<AuthModalStore>((set) => ({
  isOpen: false,
  currentView: 'login',
  onOpen: () => set({ isOpen: true }),
  onClose: () => set({ isOpen: false, currentView: 'login' }),
  setView: (view) => set({ currentView: view }),
}));
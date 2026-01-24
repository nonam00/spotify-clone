import { create } from "zustand";

export type AuthView = "login" | "register" | "forgot-password";

type AuthModalStore = {
  isOpen: boolean;
  currentView: AuthView;
  onOpen: () => void;
  onClose: () => void;
  setView: (view: AuthView) => void;
}

export const useAuthModalStore = create<AuthModalStore>((set) => ({
  isOpen: false,
  currentView: 'login',
  onOpen: () => set({ isOpen: true }),
  onClose: () => set({ isOpen: false, currentView: 'login' }),
  setView: (view: AuthView) => set({ currentView: view }),
}));
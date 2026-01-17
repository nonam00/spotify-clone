import { create } from "zustand";
import { CLIENT_API_URL } from "@/shared/config/api";
import { type UserDetails, getUserInfo } from "@/entities/user";

type AuthStore = {
  user: UserDetails | null;
  isLoading: boolean;
  error: string | null;
  isAuthenticated: boolean;
  checkAuth: () => Promise<void>;
  login: (email: string, password: string) => Promise<boolean>;
  register: (email: string, password: string, fullName: string) => Promise<boolean>;
  forgotPassword: (email: string) => Promise<boolean>;
  logout: () => Promise<void>;
  cleanError: () => void;
}

export const useAuthStore = create<AuthStore>((set, get) => ({
  user: null,
  isLoading: true,
  error: null,
  isAuthenticated: false,

  checkAuth: async () => {
    set({ isLoading: true, error: null });
    try {
      const userInfo = await getUserInfo();
      set({
        user: userInfo,
        isLoading: false,
        isAuthenticated: true,
        error: null,
      });
    } catch (error) {
      const errorMessage =
        error instanceof Error ? error.message : "Failed to authenticate";
      const isUnauthorized = errorMessage === "Unauthorized";

      set({
        error: isUnauthorized ? null : errorMessage,
        isLoading: false,
        isAuthenticated: false,
        user: null,
      });
    }
  },

  login: async (email: string, password: string) => {
    set({ isLoading: true, error: null });
    try {
      const formData = new FormData();
      formData.append("Email", email);
      formData.append("Password", password);

      const response = await fetch(`${CLIENT_API_URL}/auth/login/`, {
        method: "POST",
        credentials: "include",
        body: formData,
      });

      if (!response.ok) {
        if (response.status === 400) {
          const error = await response.json();
          set({
            error: error.detail || "Invalid email or password",
            isLoading: false,
          });
          return false;
        }
        set({
          error: "An error occurred when you tried to log in.",
          isLoading: false,
        });
        return false;
      }

      await get().checkAuth();
      return get().isAuthenticated;
    } catch (error) {
      set({
        error: error instanceof Error ? error.message : "Failed to login",
        isLoading: false,
      });
      return false;
    }
  },

  register: async (email: string, password: string, fullName: string) => {
    set({ isLoading: true, error: null });
    try {
      const formData = new FormData();
      formData.append("Email", email);
      formData.append("Password", password);
      formData.append("FullName", fullName);

      const response = await fetch(`${CLIENT_API_URL}/auth/register/`, {
        method: "POST",
        credentials: "include",
        body: formData,
      });

      if (!response.ok) {
        if (response.status === 400) {
          const error = await response.json();
          set({
            error: error.detail || "Failed to register",
            isLoading: false,
          });
          return false;
        }
        set({
          error: "An error occurred when you tried to register your account.",
          isLoading: false,
        });
        return false;
      }

      set({
        error: null,
        isLoading: false,
      });
      return true;
    } catch (error) {
      set({
        error: error instanceof Error ? error.message : "Failed to register",
        isLoading: false,
      });
      return false;
    }
  },

  forgotPassword: async (email: string) => {
    set({ isLoading: true, error: null });
    try {
      const response = await fetch(`${CLIENT_API_URL}/auth/send-restore-code?email=${email}`, {
        method: "POST",
      });

      if (!response.ok) {
        if (response.status === 400) {
          const error = await response.json();
          set({
            error: error.detail || "Failed to process password reset",
            isLoading: false,
          });
          return false;
        }
        set({
          error: "An error occurred when processing your request.",
          isLoading: false,
        });
        return false;
      }

      set({
        error: null,
        isLoading: false,
      });
      return true;
    } catch (error) {
      set({
        error: error instanceof Error ? error.message : "Failed to process password reset",
        isLoading: false,
      });
      return false;
    }
  },

  logout: async () => {
    try {
      await fetch(`${CLIENT_API_URL}/auth/logout/`, {
        method: "POST",
        credentials: "include",
      });

      set({
        user: null,
        isAuthenticated: false,
        error: null,
      });
    } catch (error) {
      console.error("Logout error:", error);
    }
  },
  cleanError: () => {
    set({
      error: null,
    })
  }
}));
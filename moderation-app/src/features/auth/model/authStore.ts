import { create } from "zustand";
import { CLIENT_API_URL } from "@/shared/config/api";
import { getModeratorInfo, type ModeratorInfo, type ModeratorPermissions } from "@/entities/moderator";

type AuthStore = {
  user: ModeratorInfo | null;
  isLoading: boolean;
  error: string | null;
  isAuthenticated: boolean;
  checkAuth: () => Promise<void>;
  login: (email: string, password: string) => Promise<boolean>;
  logout: () => Promise<void>;
  hasPermission: (permission: keyof ModeratorPermissions) => boolean;
}

export const useAuthStore = create<AuthStore>((set, get) => ({
  user: null,
  isLoading: true,
  error: null,
  isAuthenticated: false,

  checkAuth: async () => {
    set({ isLoading: true, error: null });
    try {
      const userInfo = await getModeratorInfo();

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
      const isForbidden = errorMessage === "Forbidden";

      set({
        error: isUnauthorized
          ? null
          : isForbidden
          ? "Access denied. Please use a moderator account."
          : errorMessage,
        isLoading: false,
        isAuthenticated: false,
        user: null,
      });
    }
  },

  hasPermission: (permission) => {
    const { user } = get();
    if (!user) {
      return false;
    }
    return Boolean(user.permissions?.[permission]);
  },

  login: async (email: string, password: string) => {
    set({ isLoading: true, error: null });
    try {
      const formData = new FormData();
      formData.append("Email", email);
      formData.append("Password", password);

      const response = await fetch(`${CLIENT_API_URL}/auth/moderators/login`, {
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

  logout: async () => {
    try {
      await fetch(`${CLIENT_API_URL}/auth/moderators/logout`, {
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
}));


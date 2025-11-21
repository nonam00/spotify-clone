import { create } from "zustand";
import type { ManagedUser } from "@/entities/user/model";
import { getUsersForModeration, updateUserStatus as updateUserStatusApi } from "@/entities/user/api";

type UsersStore = {
  users: ManagedUser[];
  isLoading: boolean;
  error: string | null;
  fetchUsers: () => Promise<void>;
  updateStatus: (userId: string, isActive: boolean) => Promise<void>;
};

function extractError(error: unknown, fallback: string) {
  return error instanceof Error ? error.message : fallback;
}

export const useUsersStore = create<UsersStore>((set, get) => ({
  users: [],
  isLoading: false,
  error: null,

  fetchUsers: async () => {
    set({ isLoading: true, error: null });
    try {
      const data = await getUsersForModeration();
      set({ users: data.users, isLoading: false });
    } catch (error) {
      set({ error: extractError(error, "Failed to fetch users"), isLoading: false });
      throw error;
    }
  },

  updateStatus: async (userId, isActive) => {
    set({ isLoading: true, error: null });
    try {
      await updateUserStatusApi(userId, isActive);
      await get().fetchUsers();
      set({ isLoading: false });
    } catch (error) {
      set({ error: extractError(error, "Failed to update user status"), isLoading: false });
      throw error;
    }
  },
}));
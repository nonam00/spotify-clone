import { create } from "zustand";
import {
  type ManagedUser,
  getUsersForModeration,
  updateUserStatus as updateUserStatusApi
} from "@/entities/user";

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
      const users = get().users;
      const index = users.findIndex(user => user.id === userId);
      const user = users[index];
      set({
        isLoading: false,
        users: [
          ...users.slice(0, index),
          {
            ...user,
            isActive,
          },
          ...users.slice(index + 1),
        ]
      });
    } catch (error) {
      set({ error: extractError(error, "Failed to update user status"), isLoading: false });
      throw error;
    }
  },
}));
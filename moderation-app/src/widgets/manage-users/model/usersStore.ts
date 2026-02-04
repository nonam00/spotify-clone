import { create } from "zustand";
import {
  type ManagedUser,
  getUsersForModeration,
  activateUser as activateUserApi,
  deactivateUser as deactivateUserApi,
} from "@/entities/user";

type UsersStore = {
  users: ManagedUser[];
  isLoading: boolean;
  error: string | null;
  fetchUsers: () => Promise<void>;
  activateUser: (userId: string) => Promise<void>;
  deactivateUser: (userId: string) => Promise<void>;
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

  activateUser: async (userId: string) => {
    set({ isLoading: true, error: null })
    try {
      await activateUserApi(userId);
      const users = get().users;
      const index = users.findIndex(user => user.id === userId);
      const user = users[index];
      set({
        isLoading: false,
        users: [
          ...users.slice(0, index),
          {
            ...user,
            isActive: true,
          },
          ...users.slice(index + 1),
        ],
      });
    } catch (error) {
      set({ error: extractError(error, "Failed to activate user"), isLoading: false });
      throw error;
    }
  },

  deactivateUser: async (userId: string) => {
    set({ isLoading: true, error: null })
    try {
      await deactivateUserApi(userId);
      const users = get().users;
      const index = users.findIndex(user => user.id === userId);
      const user = users[index];
      set({
        isLoading: false,
        users: [
          ...users.slice(0, index),
          {
            ...user,
            isActive: false,
          },
          ...users.slice(index + 1),
        ],
      });
    } catch (error) {
      set({ error: extractError(error, "Failed to deactivate user"), isLoading: false });
      throw error;
    }
  },
}));
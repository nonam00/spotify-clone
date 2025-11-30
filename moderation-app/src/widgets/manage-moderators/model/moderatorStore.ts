import { create } from "zustand";
import type { ModeratorSummary } from "@/entities/moderator";
import {
  createModerator as createModeratorApi,
  getModerators,
  updateModeratorPermissions as updateModeratorPermissionsApi,
  updateModeratorStatus as updateModeratorStatusApi,
  type CreateModeratorPayload,
  type UpdateModeratorPermissionsPayload,
} from "@/entities/moderator";

type ModeratorsStore = {
  moderators: ModeratorSummary[];
  isLoading: boolean;
  error: string | null;
  fetchModerators: () => Promise<void>;
  updatePermissions: (moderatorId: string, payload: UpdateModeratorPermissionsPayload) => Promise<void>;
  updateStatus: (moderatorId: string, isActive: boolean) => Promise<void>;
  createModerator: (payload: CreateModeratorPayload) => Promise<void>;
};

function extractError(error: unknown, fallback: string) {
  return error instanceof Error ? error.message : fallback;
}

export const useModeratorsStore = create<ModeratorsStore>((set, get) => ({
  moderators: [],
  isLoading: false,
  error: null,

  fetchModerators: async () => {
    set({ isLoading: true, error: null });
    try {
      const data = await getModerators();
      set({ moderators: data.moderators, isLoading: false });
    } catch (error) {
      set({ error: extractError(error, "Failed to fetch moderators"), isLoading: false });
      throw error;
    }
  },

  updatePermissions: async (moderatorId, payload) => {
    set({ isLoading: true, error: null });
    try {
      await updateModeratorPermissionsApi(moderatorId, payload);
      const moderators = get().moderators;
      const index = moderators.findIndex(moderator => moderator.id === moderatorId);
      const moderator = moderators[index];
      set({
        isLoading: false,
        moderators: [
          ...moderators.slice(0, index),
          {
            ...moderator,
            permissions: {
              ...payload
            },
          },
          ...moderators.slice(index + 1),
        ]
      });
    } catch (error) {
      set({ error: extractError(error, "Failed to update permissions"), isLoading: false });
      throw error;
    }
  },

  updateStatus: async (moderatorId, isActive) => {
    set({ isLoading: true, error: null });
    try {
      await updateModeratorStatusApi(moderatorId, isActive);
      const moderators = get().moderators;
      const index = moderators.findIndex(moderator => moderator.id === moderatorId);
      const moderator = moderators[index];
      set({
        isLoading: false,
        moderators: [
          ...moderators.slice(0, index),
          {
            ...moderator,
            isActive
          },
          ...moderators.slice(index + 1),
        ]
      });
    } catch (error) {
      set({ error: extractError(error, "Failed to update moderator status"), isLoading: false });
      throw error;
    }
  },

  createModerator: async (payload) => {
    set({ isLoading: true, error: null });
    try {
      await createModeratorApi(payload);
      await get().fetchModerators();
      set({ isLoading: false });
    } catch (error) {
      set({ error: extractError(error, "Failed to create moderator"), isLoading: false });
      throw error;
    }
  },
}));
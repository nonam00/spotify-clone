import { create } from "zustand";
import type { ModeratorSummary } from "@/entities/moderator";
import {
  createModerator as createModeratorApi,
  getModerators,
  updateModeratorPermissions as updateModeratorPermissionsApi,
  activateModerator as activateModeratorApi,
  deactivateModerator as deactivateModeratorApi,
  type CreateModeratorPayload,
  type UpdateModeratorPermissionsPayload,
} from "@/entities/moderator";

type ModeratorsStore = {
  moderators: ModeratorSummary[];
  isLoading: boolean;
  error: string | null;
  fetchModerators: () => Promise<void>;
  updatePermissions: (moderatorId: string, payload: UpdateModeratorPermissionsPayload) => Promise<void>;
  activateModerator: (moderatorId: string) => Promise<void>;
  deactivateModerator: (moderatorId: string) => Promise<void>;
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

  activateModerator: async (moderatorId: string) => {
    set({isLoading: true, error: null});
    try {
      await activateModeratorApi(moderatorId);
      const moderators = get().moderators;
      const index = moderators.findIndex(moderator => moderator.id === moderatorId);
      const moderator = moderators[index];
      set({
        isLoading: false,
        moderators: [
          ...moderators.slice(0, index),
          {
            ...moderator,
            isActive: true,
          },
          ...moderators.slice(index + 1),
        ]
      });
    } catch (error) {
      set({error: extractError(error, "Failed to activate moderator"), isLoading: false});
      throw error;
    }
  },

  deactivateModerator: async (moderatorId: string) => {
    set({isLoading: true, error: null});
    try {
      await deactivateModeratorApi(moderatorId);
      const moderators = get().moderators;
      const index = moderators.findIndex(moderator => moderator.id === moderatorId);
      const moderator = moderators[index];
      set({
        isLoading: false,
        moderators: [
          ...moderators.slice(0, index),
          {
            ...moderator,
            isActive: false
          },
          ...moderators.slice(index + 1),
        ]
      });
    } catch (error) {
      set({error: extractError(error, "Failed to deactivate moderator"), isLoading: false});
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
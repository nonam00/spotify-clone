import { authFetchClient, MODERATORS_API_URL } from "@/shared/config/api.ts";
import type { ModeratorPermissions } from "@/entities/moderator/model";

export interface ModeratorInfo {
  id: string;
  email: string;
  fullName: string;
  isActive: boolean;
  permissions: ModeratorPermissions;
}

export async function getModeratorInfo(): Promise<ModeratorInfo> {
  const response = await authFetchClient(`${MODERATORS_API_URL}/info`, {
    method: "GET",
  });

  if (!response.ok) {
    if (response.status === 401) {
      throw new Error("Unauthorized");
    }
    if (response.status === 403) {
      throw new Error("Forbidden");
    }
    throw new Error("Failed to fetch moderator info");
  }

  return response.json();
}


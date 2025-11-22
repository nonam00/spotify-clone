import { authFetchClient, MODERATORS_API_URL } from "@/shared/config/api.ts";
import type { UserListVm } from "@/entities/user/model";
import type { SongListVm } from "@/entities/song/model";

function getErrorMessage(status: number, defaultMessage: string) {
  if (status === 401) {
    return "Unauthorized";
  }
  if (status === 403) {
    return "Forbidden";
  }
  return defaultMessage;
}

export async function getUsersForModeration(): Promise<UserListVm> {
  const response = await authFetchClient(`${MODERATORS_API_URL}/users`, {
    method: "GET",
  });

  if (!response.ok) {
    throw new Error(getErrorMessage(response.status, "Failed to fetch users"));
  }

  return response.json();
}

export async function updateUserStatus(userId: string, isActive: boolean): Promise<void> {
  const response = await authFetchClient(`${MODERATORS_API_URL}/users/${userId}/status`, {
    method: "PUT",
    headers: { "content-type": "application/json" },
    body: JSON.stringify({ isActive }),
  });

  if (!response.ok) {
    throw new Error(getErrorMessage(response.status, "Failed to update user status"));
  }
}

export async function getUserSongs(userId: string): Promise<SongListVm> {
  const response = await authFetchClient(`${MODERATORS_API_URL}/users/${userId}/songs`, {
    method: "GET",
  });

  if (!response.ok) {
    throw new Error(getErrorMessage(response.status, "Failed to fetch user songs"));
  }

  return response.json();
}


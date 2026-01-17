import {authFetchClient, CLIENT_API_URL} from "@/shared/config/api";
import type {UserListVm} from "../model";

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
  const response = await authFetchClient(`${CLIENT_API_URL}/moderators/users`, {
    method: "GET",
  });

  if (!response.ok) {
    throw new Error(getErrorMessage(response.status, "Failed to fetch users"));
  }

  return response.json();
}

export async function updateUserStatus(userId: string, isActive: boolean): Promise<void> {
  const response = await authFetchClient(`${CLIENT_API_URL}/moderators/users/${userId}/status`, {
    method: "PUT",
    headers: { "content-type": "application/json" },
    body: JSON.stringify({ isActive }),
  });

  if (!response.ok) {
    throw new Error(getErrorMessage(response.status, "Failed to update user status"));
  }
}
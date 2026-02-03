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

export async function activateUser(userId: string): Promise<void> {
  const response = await authFetchClient(`${CLIENT_API_URL}/moderators/users/${userId}/activate`, {
    method: "PUT",
  });

  if (!response.ok) {
    throw new Error(getErrorMessage(response.status, "Failed to activate user"));
  }
}

export async function deactivateUser(userId: string): Promise<void> {
  const response = await authFetchClient(`${CLIENT_API_URL}/moderators/users/${userId}/deactivate`, {
    method: "PUT",
  });

  if (!response.ok) {
    throw new Error(getErrorMessage(response.status, "Failed to deactivate user"));
  }
}
import {authFetchClient, CLIENT_API_URL} from "@/shared/config/api";
import type {ModeratorListVm, ModeratorInfo} from "../model";

export type UpdateModeratorPermissionsPayload = {
  canManageUsers: boolean;
  canManageContent: boolean;
  canViewReports: boolean;
  canManageModerators: boolean;
}

export type CreateModeratorPayload = {
  email: string;
  fullName: string;
  password: string;
  isSuper: boolean;
}

function getErrorMessage(status: number, defaultMessage: string) {
  if (status === 401) {
    return "Unauthorized";
  }
  if (status === 403) {
    return "Forbidden";
  }
  return defaultMessage;
}

export async function getModerators(): Promise<ModeratorListVm> {
  const response = await authFetchClient(`${CLIENT_API_URL}/moderators`, {
    method: "GET",
  });

  if (!response.ok) {
    throw new Error(getErrorMessage(response.status, "Failed to fetch moderators"));
  }

  return response.json();
}

export async function updateModeratorPermissions(
  moderatorId: string,
  payload: UpdateModeratorPermissionsPayload
): Promise<void> {
  const response = await authFetchClient(`${CLIENT_API_URL}/moderators/${moderatorId}/permissions`, {
    method: "PUT",
    headers: { "content-type": "application/json" },
    body: JSON.stringify(payload),
  });

  if (!response.ok) {
    throw new Error(getErrorMessage(response.status, "Failed to update permissions"));
  }
}

export async function activateModerator(moderatorId: string): Promise<void> {
  const response = await authFetchClient(`${CLIENT_API_URL}/moderators/${moderatorId}/activate`, {
    method: "PUT"
  });

  if (!response.ok) {
    throw new Error(getErrorMessage(response.status, "Failed to activate moderator"));
  }
}

export async function deactivateModerator(moderatorId: string): Promise<void> {
  const response = await authFetchClient(`${CLIENT_API_URL}/moderators/${moderatorId}/deactivate`, {
    method: "PUT"
  });

  if (!response.ok) {
    throw new Error(getErrorMessage(response.status, "Failed to deactivate moderator"));
  }
}

export async function updateModeratorStatus(moderatorId: string, isActive: boolean): Promise<void> {
  const response = await authFetchClient(`${CLIENT_API_URL}/moderators/${moderatorId}/status`, {
    method: "PUT",
    headers: { "content-type": "application/json" },
    body: JSON.stringify({ isActive }),
  });

  if (!response.ok) {
    throw new Error(getErrorMessage(response.status, "Failed to update moderator status"));
  }
}

export async function createModerator(payload: CreateModeratorPayload): Promise<void> {
  const formData = new FormData();
  formData.append("Email", payload.email);
  formData.append("FullName", payload.fullName);
  formData.append("Password", payload.password);
  formData.append("IsSuper", String(payload.isSuper));

  const response = await authFetchClient(`${CLIENT_API_URL}/moderators/register`, {
    method: "POST",
    body: formData,
  });

  if (!response.ok) {
    const message =
      response.status === 400
        ? (await response.json().catch(() => ({}))).detail ?? "Failed to create moderator"
        : getErrorMessage(response.status, "Failed to create moderator");
    throw new Error(message);
  }
}

export async function getModeratorInfo(): Promise<ModeratorInfo> {
  const response = await authFetchClient(`${CLIENT_API_URL}/moderators/info`, {
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
import {authFetchClient, CLIENT_API_URL} from "@/shared/config/api";
import type {SongListVm} from "../model";

export async function getUnpublishedSongs(): Promise<SongListVm> {
  const response = await authFetchClient(`${CLIENT_API_URL}/moderators/songs/unpublished`, {
    method: "GET",
  });

  if (!response.ok) {
    if (response.status === 401) {
      throw new Error("Unauthorized");
    }
    if (response.status === 403) {
      throw new Error("Forbidden: You must be a moderator or administrator");
    }
    throw new Error("Failed to fetch unpublished songs");
  }

  return response.json();
}

export async function publishSong(songId: string): Promise<void> {
  const response = await authFetchClient(`${CLIENT_API_URL}/moderators/songs/publish/${songId}`, {
    method: "PUT",
  });

  if (!response.ok) {
    if (response.status === 401) {
      throw new Error("Unauthorized");
    }
    if (response.status === 403) {
      throw new Error("Forbidden: You must be a moderator or administrator");
    }
    throw new Error("Failed to publish song");
  }
}

export async function publishSongs(songIds: string[]): Promise<void> {
  const response = await authFetchClient(`${CLIENT_API_URL}/moderators/songs/publish`, {
    method: "PUT",
    headers: { "content-type": "application/json" },
    body: JSON.stringify({ songIds }),
  });

  if (!response.ok) {
    if (response.status === 401) {
      throw new Error("Unauthorized");
    }
    if (response.status === 403) {
      throw new Error("Forbidden: You must be a moderator or administrator");
    }
    throw new Error("Failed to publish songs");
  }
}

export async function unpublishSong(songId: string): Promise<void> {
  const response = await authFetchClient(`${CLIENT_API_URL}/moderators/songs/unpublish/${songId}`, {
    method: "PUT",
  });

  if (!response.ok) {
    if (response.status === 401) {
      throw new Error("Unauthorized");
    }
    if (response.status === 403) {
      throw new Error("Forbidden: You must be a moderator or administrator");
    }
    throw new Error("Failed to publish song");
  }
}

export async function deleteSong(songId: string): Promise<void> {
  const response = await authFetchClient(`${CLIENT_API_URL}/moderators/songs/${songId}`, {
    method: "DELETE",
  });

  if (!response.ok) {
    if (response.status === 401) {
      throw new Error("Unauthorized");
    }
    if (response.status === 403) {
      throw new Error("Forbidden: You must be a moderator or administrator");
    }
    throw new Error("Failed to delete song");
  }
}

export async function deleteSongs(songIds: string[]): Promise<void> {
  const response = await authFetchClient(`${CLIENT_API_URL}/moderators/songs`, {
    method: "DELETE",
    headers: { "content-type": "application/json" },
    body: JSON.stringify({ songIds }),
  });

  if (!response.ok) {
    if (response.status === 401) {
      throw new Error("Unauthorized");
    }
    if (response.status === 403) {
      throw new Error("Forbidden: You must be a moderator or administrator");
    }
    throw new Error("Failed to delete songs");
  }
}

export async function getUserSongs(userId: string): Promise<SongListVm> {
  const response = await authFetchClient(`${CLIENT_API_URL}/moderators/users/${userId}/songs`, {
    method: "GET",
  });

  if (!response.ok) {
    throw new Error("Failed to fetch user songs");
  }

  return response.json();
}
import { authFetchClient, CLIENT_API_URL } from "@/shared/config/api";
import type { SongListVm } from "@/entities/song";

export async function getUnpublishedSongs(): Promise<SongListVm> {
  const response = await authFetchClient(`${CLIENT_API_URL}/songs/unpublished`, {
    method: "GET",
    credentials: "include",
  });

  if (!response.ok) {
    throw new Error("Failed to fetch unpublished songs");
  }

  return response.json();
}

export async function publishSong(songId: string): Promise<void> {
  const response = await authFetchClient(`${CLIENT_API_URL}/songs/publish/${songId}`, {
    method: "PUT",
    credentials: "include",
  });

  if (!response.ok) {
    throw new Error("Failed to publish song");
  }
}

export async function publishSongs(songIds: string[]): Promise<void> {
  const response = await authFetchClient(`${CLIENT_API_URL}/songs/publish`, {
    method: "PUT",
    headers: { "content-type": "application/json" },
    body: JSON.stringify({ songIds }),
    credentials: "include",
  });

  if (!response.ok) {
    throw new Error("Failed to publish songs");
  }
}

export async function deleteSong(songId: string): Promise<void> {
  const response = await authFetchClient(`${CLIENT_API_URL}/songs/${songId}`, {
    method: "DELETE",
    credentials: "include",
  });

  if (!response.ok) {
    throw new Error("Failed to delete song");
  }
}

export async function deleteSongs(songIds: string[]): Promise<void> {
  const response = await authFetchClient(`${CLIENT_API_URL}/songs`, {
    method: "DELETE",
    headers: { "content-type": "application/json" },
    body: JSON.stringify({ songIds }),
    credentials: "include",
  });

  if (!response.ok) {
    throw new Error("Failed to delete songs");
  }
}


import { authFetchClient, CLIENT_API_URL } from "@/shared/config/api";
import { Song } from "../model";

export async function createSongRecord(data: {
  title: string;
  author: string;
  imageId: string;
  audioId: string;
}): Promise<boolean> {
  try {
    const response = await authFetchClient(`${CLIENT_API_URL}/songs`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(data),
    });
    return response.ok;
  } catch (error) {
    console.error("Error creating song record:", error);
    return false;
  }
}

export async function getSongById(id: string, abortController?: AbortController): Promise<Song | null> {
  try {
    const response = await fetch(`${CLIENT_API_URL}/songs/${id}`, {
      signal: abortController?.signal,
    });

    const data = await response.json();

    if (!response.ok) {
      throw new Error("Failed to fetch song", data);
    }

    return data;
  } catch (error) {
    console.error("Error getting song record:", error);
    return null;
  }
}
import { authFetchClient, CLIENT_API_URL } from "@/shared/config/api";
import { LyricsSegment } from "../model";

export async function getLyricsBySongId(songId: string, signal?: AbortSignal): Promise<LyricsSegment[] | null> {
  try {
    const response = await authFetchClient(`${CLIENT_API_URL}/songs/${songId}/lyrics`, {
      signal
    });

    if (!response.ok) throw new Error("Failed to load lyrics");
    const data = await response.json();

    if (!response.ok) {
      throw new Error("Failed to fetch lyrics", data);
    }

    const lyricsSegments: LyricsSegment[] = data as LyricsSegment[];
    return lyricsSegments.sort((a, b) => a.order - b.order);
  } catch (error) {
    console.error("Error getting lyrics:", error);
    return null;
  }
}
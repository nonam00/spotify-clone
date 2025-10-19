import {authFetchClient, CLIENT_API_URL} from "@/helpers/api";

export async function createSongRecord(data: {
  title: string;
  author: string;
  imageId: string;
  audioId: string;
}): Promise<boolean> {
  try {
    const response = await authFetchClient(`${CLIENT_API_URL}/songs`, {
      method: "POST",
      headers: { "content-type": "application/json" },
      body: JSON.stringify(data),
      credentials: "include",
    });
    return response.ok;
  } catch (error) {
    console.error("Error creating song record:", error);
    return false;
  }
}
import {authFetchClient, CLIENT_API_URL} from "@/shared/config/api";

export async function addLikedSong(songId: string): Promise<boolean> {
  try {
    const response = await authFetchClient(
      `${CLIENT_API_URL}/users/songs/${songId}`,
      {
        method: "POST",
      }
    );
    return response.ok;
  } catch (error) {
    console.error("Error on adding song to liked", error);
    return false;
  }
}

export async function checkLikedSong(
  songId: string,
  abortController: AbortController
): Promise<boolean> {
  try {
    const response = await authFetchClient(
      `${CLIENT_API_URL}/users/songs/${songId}`,
      {
        method: "GET",
        signal: abortController.signal,
      }
    );
    return response.ok;
  } catch (error) {
    console.error("Error on checking if song liked", error);
    return false;
  }
}

export async function deleteLikedSong(songId: string): Promise<boolean> {
  try {
    const response = await authFetchClient(
      `${CLIENT_API_URL}/users/songs/${songId}`,
      {
        method: "DELETE",
      }
    );
    return response.ok;
  } catch (error) {
    console.error("Error on removing song from liked", error);
    return false;
  }
}
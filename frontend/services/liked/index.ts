import {authFetchClient, CLIENT_API_URL} from "@/helpers/api";

export async function addLikedSong(songId: string): Promise<boolean> {
  try {
    const response = await authFetchClient(`${CLIENT_API_URL}/users/songs/${songId}`, {
      method: 'POST',
      credentials: 'include'
    });
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
    const response = await authFetchClient(`${CLIENT_API_URL}/users/songs/${songId}`, {
      method: "GET",
      credentials: "include",
      signal: abortController.signal
    });
    return response.ok;
  } catch (error) {
    console.error("Error on checking if song liked", error);
    return false;
  }
}

export async function deleteLikedSong(songId: string) {
  try {
    const response = await authFetchClient(`${CLIENT_API_URL}/users/songs/${songId}`, {
      method: "DELETE",
      credentials: "include"
    });
    return response.ok;
  } catch (error) {
    console.error("Error on removing song from liked", error);
    return false;
  }
}

import {authFetchClient, CLIENT_API_URL} from "@/helpers/api";

export async function addLikedSong(songId: string) {
  return await authFetchClient(`${CLIENT_API_URL}/users/songs/${songId}`, {
    method: 'POST',
    credentials: 'include'
  });
}

export async function checkLikedSong(
  songId: string,
  abortController: AbortController
) {
  return await authFetchClient(`${CLIENT_API_URL}/users/songs/${songId}`, {
    method: "GET",
    credentials: "include",
    signal: abortController.signal
  })
}

export async function deleteLikedSong(songId: string) {
  return await authFetchClient(`${CLIENT_API_URL}/users/songs/${songId}`, {
    method: "DELETE",
    credentials: "include"
  });
}

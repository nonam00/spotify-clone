import {authFetchClient, CLIENT_API_URL} from "@/helpers/api";

export async function addSongToPlaylist (
  playlistId: string,
  songId: string
) {
  return await authFetchClient(`${CLIENT_API_URL}/playlists/${playlistId}/songs/${songId}/`, {
      method: "POST",
      credentials: "include",
    }
  );
}
export async function createPlaylist() {
  return await authFetchClient(`${CLIENT_API_URL}/playlists/`, {
    method: "POST",
    credentials: "include",
  });
}

export async function updatePlaylist(id: string, form: FormData) {
  return await authFetchClient(`${CLIENT_API_URL}/playlists/${id}`, {
    method: "PUT",
    credentials: "include",
    body: form
  })
}

export async function deletePlaylist(id: string) {
  return await authFetchClient(`${CLIENT_API_URL}/playlists/${id}/`, {
    method: "DELETE",
    credentials: "include",
  });
}

export async function removeSongFromPlaylist(
  playlistId: string,
  songId: string
) {
  return await authFetchClient(`${CLIENT_API_URL}/playlists/${playlistId}/songs/${songId}/`, {
    method: "DELETE",
    credentials: "include"
  });
}

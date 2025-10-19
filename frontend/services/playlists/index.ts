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

export async function updatePlaylist(id: string, data: {
  title: string,
  description: string | null,
  imageId: string | null,
}): Promise<boolean> {
  try {
    const response = await authFetchClient(`${CLIENT_API_URL}/playlists/${id}`, {
      method: "PUT",
      headers: {
        "Content-Type": "application/json",
      },
      credentials: "include",
      body: JSON.stringify(data),
    });
    return response.ok;
  } catch (error) {
    console.error("Error updating playlist", error);
    return false
  }
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

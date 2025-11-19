import {authFetchClient, CLIENT_API_URL} from "@/helpers/api";

export async function addSongsToPlaylist(
  playlistId: string,
  songIds: string[],
): Promise<boolean> {
  try {
    const response = await authFetchClient(`${CLIENT_API_URL}/playlists/${playlistId}/songs`, {
      method: "POST",
      credentials: "include",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ songIds } ),
    });
    return response.ok;
  } catch (error) {
    console.error("Error on adding songs to playlist", error);
    return false;
  }
}

export async function createPlaylist(): Promise<boolean> {
  try {
    const response = await authFetchClient(`${CLIENT_API_URL}/playlists/`, {
      method: "POST",
      credentials: "include",
    });
    return response.ok;
  } catch (error) {
    console.error("Error on creating playlist", error);
    return false;
  }
}

export async function updatePlaylist(
  id: string,
  data: {
    title: string,
    description: string | null,
    imageId: string | null,
  },
): Promise<boolean> {
  try {
    const response = await authFetchClient(`${CLIENT_API_URL}/playlists/${id}`, {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      credentials: "include",
      body: JSON.stringify(data),
    });
    return response.ok;
  } catch (error) {
    console.error("Error updating playlist", error);
    return false;
  }
}

export async function deletePlaylist(id: string) {
  try {
    const response = await authFetchClient(`${CLIENT_API_URL}/playlists/${id}/`, {
      method: "DELETE",
      credentials: "include",
    });
    return response.ok;
  } catch (error) {
    console.error("Error on deleting playlist", error);
    return false;
  }
}

export async function removeSongFromPlaylist(
  playlistId: string,
  songId: string
) {
  try {
    const response = await authFetchClient(`${CLIENT_API_URL}/playlists/${playlistId}/songs/${songId}/`, {
      method: "DELETE",
      credentials: "include"
    });
    return response.ok;
  } catch (error) {
    console.error("Error on removing song from playlist", error);
    return false;
  }
}

export async function reorderPlaylistSongs(
  playlistId: string,
  songIds: string[]
): Promise<boolean> {
  try {
    const response = await authFetchClient(`${CLIENT_API_URL}/playlists/${playlistId}/songs/reorder`, {
      method: "PUT",
      credentials: "include",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ songIds }),
    });
    return response.ok;
  } catch (error) {
    console.error("Error reordering songs in playlist", error);
    return false;
  }
}

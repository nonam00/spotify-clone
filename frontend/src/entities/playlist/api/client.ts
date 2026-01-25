import { authFetchClient, CLIENT_API_URL } from "@/shared/config/api";
import {Playlist} from "@/entities/playlist";

export async function createPlaylist(): Promise<boolean> {
  try {
    const response = await authFetchClient(`${CLIENT_API_URL}/playlists/`, {
      method: "POST",
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
    title: string;
    description: string | null;
    imageId: string | null;
  }
): Promise<boolean> {
  try {
    const response = await authFetchClient(`${CLIENT_API_URL}/playlists/${id}`, {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(data),
    });
    return response.ok;
  } catch (error) {
    console.error("Error updating playlist", error);
    return false;
  }
}

export async function deletePlaylist(id: string): Promise<boolean> {
  try {
    const response = await authFetchClient(`${CLIENT_API_URL}/playlists/${id}/`, {
      method: "DELETE",
    });
    return response.ok;
  } catch (error) {
    console.error("Error on deleting playlist", error);
    return false;
  }
}

export async function addSongToPlaylist(
  playlistId: string,
  songId: string
): Promise<boolean> {
  try {
    const response = await authFetchClient(
      `${CLIENT_API_URL}/playlists/${playlistId}/songs/${songId}`,
      {
        method: "POST",
      }
    );
    return response.ok;
  } catch (error) {
    console.error("Error on adding song to playlist", error);
    return false;
  }
}

export async function addSongsToPlaylist(
  playlistId: string,
  songIds: string[]
): Promise<boolean> {
  try {
    const response = await authFetchClient(
      `${CLIENT_API_URL}/playlists/${playlistId}/songs`,
      {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ songIds }),
      }
    );
    return response.ok;
  } catch (error) {
    console.error("Error on adding songs to playlist", error);
    return false;
  }
}

export async function removeSongFromPlaylist(
  playlistId: string,
  songId: string
): Promise<boolean> {
  try {
    const response = await authFetchClient(
      `${CLIENT_API_URL}/playlists/${playlistId}/songs/${songId}/`,
      {
        method: "DELETE",
      }
    );
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
    const response = await authFetchClient(
      `${CLIENT_API_URL}/playlists/${playlistId}/songs/reorder`,
      {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ songIds }),
      }
    );
    return response.ok;
  } catch (error) {
    console.error("Error reordering like-button in playlist", error);
    return false;
  }
}

export async function getPlaylistsWithoutSong(songId: string): Promise<Playlist[]> {
  try {
    const response = await authFetchClient(`${CLIENT_API_URL}/playlists/without-song/${songId}`);

    const data = await response.json();

    if (!response.ok) {
      console.error("Error getting playlists", data);
      return [];
    }

    return data.playlists as Playlist[];
  } catch (error) {
    console.error("Error getting playlists", error);
    return [];
  }
}
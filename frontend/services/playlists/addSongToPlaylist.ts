import Cookies from "js-cookie";

import { API_URL } from "@/api/http"

const addSongToPlaylist = async (
  playlistId: string,
  songId: string
) => {
  const response = await fetch(
    `${API_URL}/playlists/${playlistId}/songs/${songId}/`,
    {
      headers: {
        "x-xsrf-token": Cookies.get(".AspNetCore.Xsrf") ?? "",
      },
      method: "POST",
      credentials: "include",
    }
  );

  return response;
}

export default addSongToPlaylist;

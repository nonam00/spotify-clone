import Cookies from "js-cookie";

import { API_URL } from "@/api/http"

const removeSongFromPlaylist = async (
  playlistId: string,
  songId: string
) => {
  const response = await fetch(
    `${API_URL}/playlists/${playlistId}/songs/${songId}/`,
    {
      headers: {
        "x-xsrf-token": Cookies.get(".AspNetCore.Xsrf") ?? "",
      },
      method: "DELETE",
      credentials: "include"
    }
  );

  return response;
}

export default removeSongFromPlaylist;

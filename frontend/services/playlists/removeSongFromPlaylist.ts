import Cookies from "js-cookie";


import {CLIENT_API_URL} from "@/helpers/api";

const removeSongFromPlaylist = async (
  playlistId: string,
  songId: string
) => {
    return await fetch(
      `${CLIENT_API_URL}/playlists/${playlistId}/songs/${songId}/`,
      {
          headers: {
              "x-xsrf-token": Cookies.get(".AspNetCore.Xsrf") ?? "",
          },
          method: "DELETE",
          credentials: "include"
      }
  );
}

export default removeSongFromPlaylist;

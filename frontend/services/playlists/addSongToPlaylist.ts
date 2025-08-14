import Cookies from "js-cookie";


import {CLIENT_API_URL} from "@/helpers/api";

const addSongToPlaylist = async (
  playlistId: string,
  songId: string
) => {
    return await fetch(
      `${CLIENT_API_URL}/playlists/${playlistId}/songs/${songId}/`,
      {
          headers: {
              "x-xsrf-token": Cookies.get(".AspNetCore.Xsrf") ?? "",
          },
          method: "POST",
          credentials: "include",
      }
  );
}

export default addSongToPlaylist;

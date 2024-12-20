import Cookies from "js-cookie";

import {API_URL} from "@/api/http";

const createPlaylist = async () => {
  return await fetch(`${API_URL}/playlists/`, {
    headers: {
      "x-xsrf-token": Cookies.get(".AspNetCore.Xsrf") ?? "",
    },
    method: "POST",
    credentials: "include",
  });
}

export default createPlaylist;

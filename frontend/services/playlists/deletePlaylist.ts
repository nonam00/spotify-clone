import Cookies from "js-cookie";

import {API_URL} from "@/api/http";

const deletePlaylistRequest = async (id: string) => {
  return await fetch(`${API_URL}/playlists/${id}/`, {
    headers: {
      "x-xsrf-token": Cookies.get(".AspNetCore.Xsrf") ?? "",
    },
    method: "DELETE",
    credentials: "include",
  });
}

export default deletePlaylistRequest;

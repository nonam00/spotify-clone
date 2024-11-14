import Cookies from "js-cookie";

import {API_URL} from "@/api/http";

const deleteLikedSong = async (songId: string) => {
  return await fetch(`${API_URL}/users/songs/${songId}`, {
    headers: {
      "x-xsrf-token": Cookies.get(".AspNetCore.Xsrf") ?? "",
    },
    method: "DELETE",
    credentials: "include"
  });
}

export default deleteLikedSong;

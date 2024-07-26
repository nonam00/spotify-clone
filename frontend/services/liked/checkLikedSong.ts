import Cookies from "js-cookie";

import { API_URL } from "@/api/http";

const checkLikedSong = async (songId: string) => {
  const response = await fetch(`${API_URL}/users/songs/${songId}`, {
    headers: {
      "x-xsrf-token": Cookies.get(".AspNetCore.Xsrf") ?? "",
    },
    method: "GET",
    credentials: "include"
  });

  return response
}

export default checkLikedSong;

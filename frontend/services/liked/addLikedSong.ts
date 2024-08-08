import Cookies from "js-cookie";

import { API_URL } from "@/api/http";

const addLikedSong = async (songId: string) => {
  const response = await fetch(`${API_URL}/users/songs/${songId}`, {
    headers: {
      "x-xsrf-token": Cookies.get(".AspNetCore.Xsrf") ?? "",
    },
    method: 'POST',
    credentials: 'include'
  })

  return response;
}

export default addLikedSong;

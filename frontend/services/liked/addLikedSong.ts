import Cookies from "js-cookie";

import {CLIENT_API_URL} from "@/api/http";

const addLikedSong = async (songId: string) => {
  return await fetch(`${CLIENT_API_URL}/users/songs/${songId}`, {
    headers: {
      "x-xsrf-token": Cookies.get(".AspNetCore.Xsrf") ?? "",
    },
    method: 'POST',
    credentials: 'include'
  });
}

export default addLikedSong;

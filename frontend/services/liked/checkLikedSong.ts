import Cookies from "js-cookie";


import {CLIENT_API_URL} from "@/helpers/api";

const checkLikedSong = async (
  songId: string,
  abortController: AbortController
) => {
  return await fetch(`${CLIENT_API_URL}/users/songs/${songId}`, {
    headers: {
      "x-xsrf-token": Cookies.get(".AspNetCore.Xsrf") ?? "",
    },
    method: "GET",
    credentials: "include",
    signal: abortController.signal
  })
}

export default checkLikedSong;

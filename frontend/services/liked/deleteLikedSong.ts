import Cookies from "js-cookie";


import {CLIENT_API_URL} from "@/helpers/api";

const deleteLikedSong = async (songId: string) => {
  return await fetch(`${CLIENT_API_URL}/users/songs/${songId}`, {
    headers: {
      "x-xsrf-token": Cookies.get(".AspNetCore.Xsrf") ?? "",
    },
    method: "DELETE",
    credentials: "include"
  });
}

export default deleteLikedSong;

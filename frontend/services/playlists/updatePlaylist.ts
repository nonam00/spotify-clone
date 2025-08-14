import Cookies from "js-cookie";


import {CLIENT_API_URL} from "@/helpers/api";

const updatePlaylist = async(id: string, form: FormData) => {
  return await fetch(`${CLIENT_API_URL}/playlists/${id}`, {
    headers: {
      "x-xsrf-token": Cookies.get(".AspNetCore.Xsrf") ?? "",
    },
    method: "PUT",
    credentials: "include",
    body: form
  })
}

export default updatePlaylist;

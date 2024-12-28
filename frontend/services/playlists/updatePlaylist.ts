import Cookies from "js-cookie";

import {CLIENT_API_URL} from "@/api/http";

const updatePlaylist = async(
  id: string,
  title: string,
  description: string | null,
  imagePath: string
) => {
  const body = JSON.stringify({
    title: title,
    description: description,
    imagePath: imagePath,
  });

  return await fetch(`${CLIENT_API_URL}/playlists/${id}`, {
    headers: {
      "x-xsrf-token": Cookies.get(".AspNetCore.Xsrf") ?? "",
      "Content-Type": "application/json",
    },
    method: "PUT",
    credentials: "include",
    body: body
  })
}

export default updatePlaylist;

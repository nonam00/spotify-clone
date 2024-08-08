import Cookies from "js-cookie";

import { API_URL } from "@/api/http";

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

  const response = await fetch(`${API_URL}/playlists/${id}`, {
    headers: {
      "x-xsrf-token": Cookies.get(".AspNetCore.Xsrf") ?? "",
      "Content-Type": "application/json",
    },
    method: "PUT",
    credentials: "include",
    body: body
  });

  return response
}

export default updatePlaylist;

import Cookies from "js-cookie";

import {API_URL} from "@/api/http";

const uploadSong = async (
  title: string,
  description: string,
  songPath: string,
  imagePath: string
) => {
  const body = JSON.stringify({
    title: title,
    description: description,
    songPath: songPath,
    imagePath: imagePath
  });

  return await fetch(`${API_URL}/songs`, {
    headers: {
      "x-xsrf-token": Cookies.get(".AspNetCore.Xsrf") ?? "",
      "Content-Type": "application/json"
    },
    method: "POST",
    credentials: "include",
    body: body
  });
}

export default uploadSong;

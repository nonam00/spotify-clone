import Cookies from "js-cookie";

import {CLIENT_API_URL} from "@/api/http";

const uploadSong = async (
  title: string,
  author: string,
  songPath: string,
  imagePath: string
) => {
  const body = JSON.stringify({
    title: title,
    author: author,
    songPath: songPath,
    imagePath: imagePath
  });

  return await fetch(`${CLIENT_API_URL}/songs`, {
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

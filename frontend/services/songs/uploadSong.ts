import Cookies from "js-cookie";


import {CLIENT_API_URL} from "@/helpers/api";

const uploadSong = async (form: FormData) => {
  return await fetch(`${CLIENT_API_URL}/songs`, {
    headers: {
      "x-xsrf-token": Cookies.get(".AspNetCore.Xsrf") ?? "",
    },
    method: "POST",
    credentials: "include",
    body: form
  });
}

export default uploadSong;

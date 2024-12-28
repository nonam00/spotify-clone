import Cookies from "js-cookie";

import {CLIENT_API_URL} from "@/api/http"

const deleteFile = async (path: string) => {
  return await fetch(`${CLIENT_API_URL}/files/image/${path}`, {
    headers: {
      'x-xsrf-token': Cookies.get(".AspNetCore.Xsrf") ?? "",
    },
    method: "DELETE",
    credentials: "include",
  });
}

export default deleteFile;

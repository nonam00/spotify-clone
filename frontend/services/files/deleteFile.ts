import Cookies from "js-cookie";

import {API_URL} from "@/api/http"

const deleteFile = async (path: string) => {
  return await fetch(`${API_URL}/files/image/${path}`, {
    headers: {
      'x-xsrf-token': Cookies.get(".AspNetCore.Xsrf") ?? "",
    },
    method: "DELETE",
    credentials: "include",
  });
}

export default deleteFile;

import Cookies from "js-cookie";

import { API_URL } from "@/api/http"

const deleteFile = async (path: string) => {
  const response = await fetch(`${API_URL}/files/image/${path}`, {
    headers: {
      'x-xsrf-token': Cookies.get(".AspNetCore.Xsrf") ?? "",
    },
    method: "DELETE",
    credentials: "include",
  });

  return response;;
}

export default deleteFile;

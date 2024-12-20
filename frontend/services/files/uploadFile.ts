import Cookies from "js-cookie";

import {API_URL} from "@/api/http";

const uploadFile = async (
  form: FormData,
  type: string,
) => {
  return await fetch(`${API_URL}/files/${type}`, {
    headers: {
      "x-xsrf-token": Cookies.get(".AspNetCore.Xsrf") ?? "",
      "Content-Type": "multipart/form-data",
    },
    method: "POST",
    credentials: "include",
    body: form,
  });
}

export default uploadFile;

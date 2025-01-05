import Cookies from "js-cookie";

import {CLIENT_API_URL} from "@/api/http";

const uploadFile = async (
  form: FormData,
  type: string,
) => {
  return await fetch(`${CLIENT_API_URL}/files/${type}`, {
    headers: {
      "x-xsrf-token": Cookies.get(".AspNetCore.Xsrf") ?? "",
    },
    method: "POST",
    credentials: "include",
    body: form,
  });
}

export default uploadFile;

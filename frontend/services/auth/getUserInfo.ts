import Cookies from "js-cookie";

import {API_URL} from "@/api/http"

const getUserInfo = async () => {
  return await fetch(`${API_URL}/users/info`, {
    headers: {
      "X-Xsrf-Token": Cookies.get(".AspNetCore.Xsrf") ?? "",
      "Content-Type": "application/json",
    },
    method: "GET",
    credentials: "include"
  });
}

export default getUserInfo

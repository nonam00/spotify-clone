import Cookies from "js-cookie";


import {CLIENT_API_URL} from "@/helpers/api";

const getUserInfo = async () => {
  return await fetch(`${CLIENT_API_URL}/users/info`, {
    headers: {
      "X-Xsrf-Token": Cookies.get(".AspNetCore.Xsrf") ?? "",
      "Content-Type": "application/json",
    },
    method: "GET",
    credentials: "include"
  });
}

export default getUserInfo

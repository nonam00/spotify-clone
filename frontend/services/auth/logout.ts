import Cookies from "js-cookie";

import {CLIENT_API_URL} from "@/api/http";

const logoutRequest = async () => {
  return await fetch(`${CLIENT_API_URL}/users/logout/`, {
    headers: {
      "x-xsrf-token": Cookies.get(".AspNetCore.Xsrf") ?? "",
    },
    method: "POST",
    credentials: "include",
  });
}

export default logoutRequest;

import {CLIENT_API_URL} from "@/helpers/api";

const login = async (form: FormData) => {
  return await fetch(`${CLIENT_API_URL}/users/login/`, {
    method: "POST",
    credentials: "include",
    body: form
  });
}

export default login;

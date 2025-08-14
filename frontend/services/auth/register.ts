import {CLIENT_API_URL} from "@/helpers/api";

const register = async (form: FormData) => {
  return await fetch(`${CLIENT_API_URL}/users/register/`, {
    method: "POST",
    credentials: "include",
    body: form
  });
}

export default register;

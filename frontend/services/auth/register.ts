import { CLIENT_API_URL } from "@/api/http";

const register = async (form: FormData) => {
  const response = await fetch(`${CLIENT_API_URL}/users/register/`, {
    method: "POST",
    credentials: "include",
    body: form
  });
  return response;
}

export default register;

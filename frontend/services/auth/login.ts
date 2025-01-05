import { CLIENT_API_URL } from "@/api/http";

const login = async (form: FormData) => {
  const response = await fetch(`${CLIENT_API_URL}/users/login/`, {
    method: "POST",
    credentials: "include",
    body: form
  });

  return response;
}

export default login;

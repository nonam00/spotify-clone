import { API_URL } from "@/api/http";

const register = async (form: FormData) => {
  const response = await fetch(`${API_URL}/users/register/`, {
    method: "POST",
    credentials: "include",
    body: form
  });
  return response;
}

export default register;

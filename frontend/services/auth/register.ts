import { API_URL } from "@/api/http";

const registerRequest = async (email: string, password: string) => {
  const response = await fetch(`${API_URL}/users/register/`, {
    headers: {
      "Content-Type": "application/json",
    },
    method: "POST",
    credentials: "include",
    body: JSON.stringify({ email: email, password: password }),
  });

  return response;
}

export default registerRequest;

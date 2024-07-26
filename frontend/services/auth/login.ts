import { API_URL } from "@/api/http";

const loginRequest = async (email: string, password: string) => {
  const response = await fetch(`${API_URL}/users/login/`, {
    headers: {
      "Content-Type": "application/json",
    },
    method: "POST",
    credentials: "include",
    body: JSON.stringify({ email: email, password: password }),
  });

  return response;
}

export default loginRequest;

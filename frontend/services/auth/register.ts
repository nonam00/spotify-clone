import {API_URL} from "@/api/http";

const registerRequest = async (email: string, password: string) => {
  return await fetch(`${API_URL}/users/register/`, {
    headers: {
      "Content-Type": "application/json",
    },
    method: "POST",
    credentials: "include",
    body: JSON.stringify({email: email, password: password}),
  });
}

export default registerRequest;

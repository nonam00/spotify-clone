import { authFetchClient, CLIENT_API_URL } from "@/shared/config/api";
import type { UserDetails } from "../model";

export async function getUserInfo(): Promise<UserDetails> {
  const response = await authFetchClient(`${CLIENT_API_URL}/users/info`, {
    method: "GET",
    headers: {
      "Content-Type": "application/json",
    },
  });

  if (!response.ok) {
    if (response.status === 401) {
      throw new Error("Unauthorized");
    }
    throw new Error("Failed to fetch user info");
  }

  return response.json();
}
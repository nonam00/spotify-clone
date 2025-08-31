"use server";

import {cookies} from "next/headers";

import {SERVER_API_URL} from "@/helpers/api";

export const authFetchServer = async (input: string | URL, init: RequestInit) => {
  console.log("Server fetch")
  const response = await fetch(input, init);

  if (!response.ok && response.status === 401) {
    const cookieStore = await cookies();

    console.log("Refresh request")

    const refreshResponse = await fetch(`${SERVER_API_URL}/auth/refresh`, {
      headers: {
        Cookie: cookieStore.toString()
      },
      method: "POST",
      credentials: "include",
    });

    if (refreshResponse.ok) {
      return await fetch(input, init);
    }
  }
  return response;
};

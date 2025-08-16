"use server";

import {UserDetails} from "@/types/types";

import {SERVER_API_URL} from "@/helpers/api";
import {cookies} from "next/headers";

const getUserInfo = async (): Promise<UserDetails | null> => {
  try {
    const cookieStore = await cookies();
    const response = await fetch(`${SERVER_API_URL}/users/info`, {
      method: "GET",
      headers: {
        Cookie: cookieStore.toString()
      },
      credentials: "include"
    });

    if (response.status === 401) {
      return null;
    }

    const data = await response.json();

    if (!response.ok) {
      console.error(data);
      return null;
    }

    return data;
  } catch (error) {
    console.error(error);
    return null;
  }

}

export default getUserInfo;

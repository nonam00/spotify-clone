"use server";

import { cookies } from "next/headers";

import {SERVER_API_URL} from "@/api/http";
import { Song } from "@/types/types";

const getLikedSongs = async (): Promise<Song[]> => {
  try {
    const cookieStore = await cookies();
    const xsrf = cookieStore.get(".AspNetCore.Xsrf")?.value ?? "";
    const response = await fetch(`${SERVER_API_URL}/users/songs`, {
      headers: {
        Cookie: cookieStore.toString(),
        "x-xsrf-token": xsrf,
      },
      method: "GET",
      credentials: "include",
    });
    
    const data = await response.json();

    if (!response.ok) {
      throw new Error(data)
    }

    return data.likedSongs as Song[];
  } catch (error) {
    console.log(error);
    return [];
  }
}

export default getLikedSongs;

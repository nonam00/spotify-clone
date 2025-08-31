"use server";

import { cookies } from "next/headers";

import { Song } from "@/types/types";
import {SERVER_API_URL} from "@/helpers/api";

const getLikedSongs = async (): Promise<Song[]> => {
  try {
    const cookieStore = await cookies();
    const response = await fetch(`${SERVER_API_URL}/users/songs`, {
      headers: {
        Cookie: cookieStore.toString(),
      },
      method: "GET",
      credentials: "include",
    });
    
    const data = await response.json();

    if (!response.ok) {
      console.error(data);
      return [];
    }

    return data.likedSongs as Song[];
  } catch (error) {
    console.error(error);
    return [];
  }
}

export default getLikedSongs;

"use server";

import { cookies } from "next/headers";

import { API_URL } from "@/api/http";
import { Song } from "@/types/types";

const getLikedSongs = async (): Promise<Song[]> => {
  try {
    const xsrf = cookies().get(".AspNetCore.Xsrf")?.value ?? "";
    const response = await fetch(`${API_URL}/users/songs`, {
      headers: {
        Cookie: cookies().toString(),
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

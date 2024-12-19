"use server";

import { cookies } from "next/headers";

import { API_URL } from "@/api/http";
import { Song } from "@/types/types";

const getSongsByPlaylistId = async (
  playlistId: string
): Promise<Song[]> => {
  try {
    const cookieStore = await cookies()
    const xsrf = cookieStore.get(".AspNetCore.Xsrf")?.value ?? "";
    const response = await fetch(
      `${API_URL}/playlists/${playlistId}/songs`,
      {
        headers: {
          "x-xsrf-token": xsrf,
          Cookie: cookieStore.toString(),
        },
        method: "GET",
        credentials: "include",
      }
    );
    
    const data = await response.json();

    if (!response.ok) {
      throw new Error(data);
    }

    return data.songs as Song[];
  } catch(error: any) {
    console.log(error.message);
    return [];
  }
}

export default getSongsByPlaylistId;

"use server";

import { cookies } from "next/headers";

import { API_URL } from "@/api/http";
import { Song } from "@/types/types";

const getSongsByPlaylistId = async (
  playlistId: string
): Promise<Song[]> => {
  try {
    const xsrf = cookies().get(".AspNetCore.Xsrf")?.value ?? "";
    const response = await fetch(
      `${API_URL}/playlists/${playlistId}/songs`,
      {
        headers: {
          "x-xsrf-token": xsrf,
          Cookie: cookies().toString(),
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

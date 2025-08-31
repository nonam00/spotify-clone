"use server";

import { cookies } from "next/headers";

import { Song } from "@/types/types";
import {SERVER_API_URL} from "@/helpers/api";

const getSongsByPlaylistId = async (
  playlistId: string
): Promise<Song[]> => {
  try {
    const cookieStore = await cookies();
    const response = await fetch(`${SERVER_API_URL}/playlists/${playlistId}/songs`, {
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

    return data.songs as Song[];
  } catch(error) {
    console.error(error);
    return [];
  }
}

export default getSongsByPlaylistId;

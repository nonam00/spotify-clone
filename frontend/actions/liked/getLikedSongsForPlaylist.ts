"use server";

import { cookies } from "next/headers";

import { Song } from "@/types/types";
import {SERVER_API_URL} from "@/helpers/api";

const getLikedSongsForPlaylist = async (
  playlistId: string,
  searchString: string
): Promise<Song[]> => {
  try {
    const cookieStore = await cookies();
    const xsrf = cookieStore.get(".AspNetCore.Xsrf")?.value ?? "";
    const response = await fetch(
      `${SERVER_API_URL}/playlists/${playlistId}/liked/${searchString}`,
      {
        headers: {
          "x-xsrf-token": xsrf,
          Cookie: cookieStore.toString()
        },
        method: "GET",
        credentials: "include"
      }
    );

    const data = await response.json();

    if (!response.ok) {
      throw new Error(data);
    }

    return data?.likedSongs as Song[];
  } catch (error) {
    console.log(error);
    return [];
  }
}

export default getLikedSongsForPlaylist;


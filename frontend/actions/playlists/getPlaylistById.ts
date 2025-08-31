"use server";

import { cookies } from "next/headers";

import { Playlist } from "@/types/types";
import {SERVER_API_URL} from "@/helpers/api";

const getPlaylistById = async (id: string): Promise<Playlist | null> => {
  try {
    const cookieStore = await cookies();
    const response = await fetch(`${SERVER_API_URL}/playlists/${id}/`, {
      headers: {
        Cookie: cookieStore.toString()
      },
      method: "GET",
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

    return data as Playlist;
  } catch (error) {
    console.error(error);
    return null;
  }
}

export default getPlaylistById;

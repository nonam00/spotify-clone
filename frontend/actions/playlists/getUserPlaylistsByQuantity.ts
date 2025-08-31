"use server";

import {cookies} from "next/headers";

import {Playlist} from "@/types/types";
import getUserPlaylists from "./getUserPlaylists";
import {SERVER_API_URL} from "@/helpers/api";

const getUserPlaylistsByQuantity = async (
  quantity: number
): Promise<Playlist[]> => {
  if (quantity <= 0) {
    return await getUserPlaylists();
  }
  try {
    const cookieStore = await cookies();
    const response = await fetch(`${SERVER_API_URL}/playlists/certain/${quantity}`,
      {
        headers: {
          Cookie: cookieStore.toString(),
        },
        method: "GET",
        credentials: "include",
      }
    );

    const data = await response.json();

    if (!response.ok) {
      console.error(data);
      return [];
    }

    return data?.playlists as Playlist[];
  } catch (error) {
    console.error(error);
    return [];
  }
}

export default getUserPlaylistsByQuantity;

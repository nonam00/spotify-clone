"use server";

import { cookies } from "next/headers";

import { Playlist } from "@/types/types";
import {SERVER_API_URL} from "@/helpers/api";

const getUserPlaylists = async (): Promise<Playlist[]> => {
  try {
    const cookieStore = await cookies();
    const response = await fetch(`${SERVER_API_URL}/playlists/`, {
      headers: {
        Cookie: cookieStore.toString(),
      },
      method: "GET",
      credentials: "include",
    });

    if (response.status === 401) {
      return [];
    }

    const data = await response.json();

    if (!response.ok) {
      console.error(data);
      return [];
    }

    return data.playlists as Playlist[];
  } catch (error) {
    console.error(error);
    return [];
  }
}

export default getUserPlaylists;

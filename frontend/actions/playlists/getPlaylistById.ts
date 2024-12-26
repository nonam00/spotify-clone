"use server";

import { cookies } from "next/headers";

import { Playlist } from "@/types/types";
import {API_URL, SERVER_API} from "@/api/http";

const getPlaylistById = async (id: string): Promise<Playlist | null> => {
  try {
    const cookieStore = await cookies();
    const xsrf = cookieStore.get(".AspNetCore.Xsrf")?.value ?? "";
    const response = await fetch(`${SERVER_API}/playlists/${id}/`, {
      headers: {
        "x-xsrf-token": xsrf,
        Cookie: cookieStore.toString()
      },
      method: "GET",
      credentials: "include"
    });

    const data = await response.json();

    if (!response.ok) {
      throw new Error(data);
    }

    return data as Playlist;
  } catch (error) {
    console.log(error);
    return null;
  }
}

export default getPlaylistById;

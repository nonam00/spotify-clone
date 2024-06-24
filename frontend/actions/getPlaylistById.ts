"use server";

import $api from "@/api/http";
import { cookies } from "next/headers";

import { Playlist } from "@/types/types";

const getPlaylistById = async (id: string): Promise<Playlist | null> => {
  try {
    const { data } = await $api.get<Playlist>(`/playlists/${id}/`, {
      headers: {
        // needs to input manualy because of server actions 
        Cookie: cookies().getAll().map((cookie) => (
          `${cookie.name}=${cookie.value};`
        ))
      }
    });
    if(!data) {
      return null;
    }
    return data as Playlist;
  } catch (error) {
    console.log(error);
    return null;
  }
}

export default getPlaylistById;

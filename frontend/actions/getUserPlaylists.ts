"use server";

import { cookies } from "next/headers";

import $api from "@/api/http";
import { Playlist } from "@/types/types";

const getUserPlaylists = async (): Promise<Playlist[]> => {
  try {
    const { data } = await $api.get(`/playlists/`, {
      headers: {
        // needs to input manualy because of server actions 
        Cookie: cookies().getAll().map((cookie) => (
          `${cookie.name}=${cookie.value};`
        ))
      }
    });
    if(!(data?.playlists)) {
      return [];
    }
    return data?.playlists as Playlist[];
  } catch (error) {
    console.log(error);
    return [];
  }
}

export default getUserPlaylists;

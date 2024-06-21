"use server";

import { cookies } from "next/headers";

import $api from "@/api/http";
import { Song } from "@/types/types";

const getSongsByPlaylistId = async(playlistId: string): Promise<Song[]> => {
  try {
    const { data } = await $api.get(`playlists/${playlistId}/songs`, {
      headers: {
        // needs to input manualy because of server actions 
        Cookie: cookies().getAll().map((cookie) => (
          `${cookie.name}=${cookie.value};`
        ))
      }
    });
    
    if(!data?.songs) {
      return [];
    }
    return data.songs as Song[];
  } catch(error: any) {
    console.log(error.message);
    return [];
  }
}

export default getSongsByPlaylistId;

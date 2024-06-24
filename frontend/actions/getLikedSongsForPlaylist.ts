"use server"

import { cookies } from "next/headers";

import $api from "@/api/http";
import { Song } from "@/types/types";

const getLikedSongsForPlaylist =
  async (playlistId: string, searchString: string): Promise<Song[]> => {

  try {
    const { data } = await $api.get(`/playlists/${playlistId}/liked/${searchString}`, {
      headers: {
        // needs to input manualy because of server actions 
        Cookie: cookies().getAll().map((cookie) => (
          `${cookie.name}=${cookie.value};`
        ))
      }
    });
    if(!(data?.likedSongs)) {
      return [];
    }
    return data?.likedSongs as Song[];
  } catch (error) {
    console.log(error);
    return [];
  }
}

export default getLikedSongsForPlaylist;


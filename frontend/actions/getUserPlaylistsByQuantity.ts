import { cookies } from "next/headers";

import $api from "@/api/http";
import { Playlist } from "@/types/types";
import getUserPlaylists from "./getUserPlaylists";

const getUserPlaylistsByQuantity =
  async (quantity: number): Promise<Playlist[]> => {
  if (quantity <= 0) {
    const allPlaylists = await getUserPlaylists();
    return allPlaylists;
  }
  try {
    const { data } = await $api.get(`/playlists/certain/${quantity}`, {
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

export default getUserPlaylistsByQuantity;

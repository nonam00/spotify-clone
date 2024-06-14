"use server";

import $api from "@/api/http";
import { Song } from "@/types/types";
import getSongs from "./getSongs";

const getSongsByAny = async (searchString: string): Promise<Song[]> => {
  if (!searchString) {
    const allSongs = await getSongs();
    return allSongs;
  }
  try {
    const { data } = await $api.get(`/songs/search/${searchString}`) 
    return data?.songs;
  } catch (error) {
    console.log(error);
    return [];
  } 
}

export default getSongsByAny;

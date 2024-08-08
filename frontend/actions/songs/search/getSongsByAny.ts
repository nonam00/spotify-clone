"use server";

import { API_URL } from "@/api/http";
import { Song } from "@/types/types";
import getSongs from "../getSongs";

const getSongsByAny = async (searchString: string): Promise<Song[]> => {
  if (!searchString) {
    const allSongs = await getSongs();
    return allSongs;
  }
  try {
    const response = await fetch(`${API_URL}/songs/search/${searchString}`);
    const data = await response.json();

    if (!response.ok) {
      throw new Error(data);
    }

    return data.songs;
  } catch (error) {
    console.log(error);
    return [];
  } 
}

export default getSongsByAny;

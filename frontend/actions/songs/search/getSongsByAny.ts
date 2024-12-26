"use server";

import {API_URL, SERVER_API} from "@/api/http";
import {Song} from "@/types/types";
import getSongs from "../getSongs";

const getSongsByAny = async (searchString: string): Promise<Song[]> => {
  if (!searchString) {
    return await getSongs();
  }
  try {
    const response = await fetch(`${SERVER_API}/songs/search/${searchString}`);
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

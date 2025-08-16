"use server";

import {SearchType, Song} from "@/types/types";
import getSongs from "./getSongs";
import {SERVER_API_URL} from "@/helpers/api";

const getSongsBySearch = async (
  searchString: string,
  searchCriteria: SearchType
): Promise<Song[]> => {
  if (!searchString) {
    return await getSongs();
  }
  try {
    const response = await fetch(
      `${SERVER_API_URL}/songs/search?searchString=${searchString}&searchCriteria=${searchCriteria}`,
    );
    const data = await response.json();

    if (!response.ok) {
      console.error(data);
      return [];
    }

    return data.songs;
  } catch (error) {
    console.error(error);
    return [];
  }
}

export default getSongsBySearch;

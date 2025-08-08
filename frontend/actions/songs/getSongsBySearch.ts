"use server";

import {SERVER_API_URL} from "@/api/http";
import {SearchType, Song} from "@/types/types";
import getSongs from "./getSongs";

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
      throw new Error(data);
    }

    return data.songs;
  } catch (error) {
    console.log(error);
    return [];
  }
}

export default getSongsBySearch;

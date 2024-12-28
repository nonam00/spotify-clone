"use server";

import {SERVER_API_URL} from "@/api/http";
import {Song} from "@/types/types";
import getSongs from "../getSongs";

const getSongsByAuthor = async (author: string): Promise<Song[]> => {
  if (!author) {
    return await getSongs();
  }
  try {
    const response = await fetch(`${SERVER_API_URL}/songs/search/author/${author}`)
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

export default getSongsByAuthor;

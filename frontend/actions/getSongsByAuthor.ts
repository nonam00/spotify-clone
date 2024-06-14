"use server";

import $api from "@/api/http";
import { Song } from "@/types/types";
import getSongs from "./getSongs";

const getSongsByAuthor = async (author: string): Promise<Song[]> => {
  if (!author) {
    const allSongs = await getSongs();
    return allSongs;
  }
  try {
    const { data } = await $api.get(`/songs/search/author/${author}`) 
    return data?.songs;
  } catch (error) {
    console.log(error);
    return [];
  } 
}

export default getSongsByAuthor;

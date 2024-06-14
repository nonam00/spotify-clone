"use server"

import $api from '@/api/http';
import { Song } from "@/types/types";
import getSongs from "./getSongs";

const getSongsByTitle = async (title: string): Promise<Song[]> => {
  if (!title) {
    const allSongs = await getSongs();
    return allSongs;
  }
  try {
    const { data } = await $api.get(`songs/search/${title}`)
    return data?.songs;
  } catch (error) {
    console.log(error);
    return [];
  }
};

export default getSongsByTitle;
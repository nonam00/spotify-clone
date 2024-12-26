"use server";

import {API_URL, SERVER_API} from '@/api/http';
import {Song} from "@/types/types";
import getSongs from "../getSongs";

const getSongsByTitle = async (title: string): Promise<Song[]> => {
  if (!title) {
    return await getSongs();
  }
  try {
    const response = await fetch(`${SERVER_API}/songs/search/title/${title}`);
    const data = await response.json();

    if (!response.ok) {
      throw new Error(data);
    }

    return data.songs;
  } catch (error) {
    console.log(error);
    return [];
  }
};

export default getSongsByTitle;

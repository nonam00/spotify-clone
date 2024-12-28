"use server";

import { Song } from "@/types/types";
import {SERVER_API_URL} from "@/api/http";

const getSongs = async (): Promise<Song[]> => {
  try {
    const response = await fetch(`${SERVER_API_URL}/songs/newest`);
    const data = await response.json();

    if (!response.ok) {
      throw new Error(data);
    }

    return (data.songs) || [];
  } catch (error) {
    console.log(error);
    return [];
  }
}

export default getSongs;

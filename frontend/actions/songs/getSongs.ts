"use server";

import { Song } from "@/types/types";

import {SERVER_API_URL} from "@/helpers/api";

const getSongs = async (): Promise<Song[]> => {
  try {
    const response = await fetch(`${SERVER_API_URL}/songs/newest`);
    const data = await response.json();

    if (!response.ok) {
      console.error(data);
      return [];
    }

    return (data.songs) || [];
  } catch (error) {
    console.error(error);
    return [];
  }
}

export default getSongs;

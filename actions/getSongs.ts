"use server";

import { Song } from "@/types/types";
import $api from "@/api/http";

const getSongs = async (): Promise<Song[]> => {
  try {
    const { data } = await $api.get("/songs/get/all/");
    return (data?.songs) || [];
  } catch (error) {
    console.log(error);
    return [];
  }
}

export default getSongs;
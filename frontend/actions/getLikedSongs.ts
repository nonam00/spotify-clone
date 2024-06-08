"use server";

import $api from "@/api/http";
import { Song } from "@/types/types";
import { cookies } from "next/headers";

const getLikedSongs = async (): Promise<Song[]> => {
  // TODO: replace with safe methods
  const token = cookies().get("token");
  if(!token) {
    return [];
  }
  try {
    const { data } = await $api.get("/liked/get/", {
      headers: {
        Authorization: `Bearer ${token.value}`
      }
    });
    if(!(data?.likedSongs)) {
      return [];
    }
    console.log(data?.likedSongs);
    return data?.likedSongs as Song[];
  } catch (error) {
    console.log(error);
    return [];
  }
}

export default getLikedSongs;
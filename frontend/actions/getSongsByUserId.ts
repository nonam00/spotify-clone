"use server";

import getLikedSongs from '@/actions/getLikedSongs';
import { Song } from "@/types/types";

// TODO: replace with something else
const getSongsByUserId = async (): Promise<Song[]> => {
  try {
    const data = getLikedSongs();
    return data; 
  } catch (error) {
    console.log(error);
    return [];
  }
};

export default getSongsByUserId;
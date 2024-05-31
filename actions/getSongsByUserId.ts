"use server";

import getLikedSongs from '@/actions/getLikedSongs';
import { Song } from "@/types/types";

const getSongsByUserId = async (): Promise<Song[]> => {
  // TODO: replace with own API
  try {
    const data = getLikedSongs();
    return data; 
  } catch (error) {
    console.log(error);
    return [];
  }
};

export default getSongsByUserId;
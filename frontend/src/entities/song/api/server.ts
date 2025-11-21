"use server";

import {cookies} from "next/headers";
import {SERVER_API_URL} from "@/shared/config/api";
import type {SearchType, Song} from "../model";

export async function getSongs(): Promise<Song[]> {
  "use server";

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

export async function getSongsBySearch(
  searchString: string,
  searchCriteria: SearchType
): Promise<Song[]> {
  "use server";

  if (!searchString) {
    return await getSongs();
  }
  try {
    const response = await fetch(
      `${SERVER_API_URL}/songs/search?searchString=${searchString}&searchCriteria=${searchCriteria}`,
    );
    const data = await response.json();

    if (!response.ok) {
      console.error(data);
      return [];
    }

    return data.songs;
  } catch (error) {
    console.error(error);
    return [];
  }
}

export async function getSongsByPlaylistId(playlistId: string): Promise<Song[]> {
  "use server";

  try {
    const cookieStore = await cookies();
    const response = await fetch(`${SERVER_API_URL}/playlists/${playlistId}/songs`, {
      headers: {
        Cookie: cookieStore.toString(),
      },
      method: "GET",
      credentials: "include",
    });

    const data = await response.json();

    if (!response.ok) {
      console.error(data);
      return [];
    }

    return data.songs as Song[];
  } catch(error) {
    console.error(error);
    return [];
  }
}

export async function getLikedSongs(): Promise<Song[]> {
  "use server";

  try {
    const cookieStore = await cookies();
    const response = await fetch(`${SERVER_API_URL}/users/songs`, {
      headers: {
        Cookie: cookieStore.toString(),
      },
      method: "GET",
      credentials: "include",
    });

    const data = await response.json();

    if (!response.ok) {
      console.error(data);
      return [];
    }

    return data.songs as Song[];
  } catch (error) {
    console.error(error);
    return [];
  }
}

export async function getLikedSongsForPlaylist(
  playlistId: string,
  searchString: string
): Promise<Song[]> {
  "use server";

  try {
    const cookieStore = await cookies();
    const response = await fetch(`${SERVER_API_URL}/playlists/${playlistId}/liked/${searchString}`,{
      headers: {
        Cookie: cookieStore.toString()
      },
      method: "GET",
      credentials: "include"
    });

    const data = await response.json();

    if (!response.ok) {
      console.error(data);
      return [];
    }

    return data?.songs as Song[];
  } catch (error) {
    console.error(error);
    return [];
  }
}
"use server";

import { cookies } from "next/headers";
import {SERVER_API_URL} from "@/shared/config/api";
import type {Playlist} from "../model";

export async function getPlaylistById(id: string): Promise<Playlist | null> {
  "use server";

  try {
    const cookieStore = await cookies();
    const response = await fetch(`${SERVER_API_URL}/playlists/${id}/`, {
      headers: {
        Cookie: cookieStore.toString()
      },
      method: "GET",
      credentials: "include"
    });

    if (response.status === 401) {
      return null;
    }

    const data = await response.json();

    if (!response.ok) {
      console.error(data);
      return null;
    }

    return data as Playlist;
  } catch (error) {
    console.error(error);
    return null;
  }
}

export async function getUserPlaylists(): Promise<Playlist[]> {
  "use server";

  try {
    const cookieStore = await cookies();
    const response = await fetch(`${SERVER_API_URL}/playlists/`, {
      headers: {
        Cookie: cookieStore.toString(),
      },
      method: "GET",
      credentials: "include",
    });

    if (response.status === 401) {
      return [];
    }

    const data = await response.json();

    if (!response.ok) {
      console.error(data);
      return [];
    }

    return data.playlists as Playlist[];
  } catch (error) {
    console.error(error);
    return [];
  }
}

export async function getUserPlaylistsByQuantity(quantity: number): Promise<Playlist[]> {
  "use server";

  if (quantity <= 0) {
    return await getUserPlaylists();
  }

  try {
    const cookieStore = await cookies();
    const response = await fetch(`${SERVER_API_URL}/playlists/certain/${quantity}`,
      {
        headers: {
          Cookie: cookieStore.toString(),
        },
        method: "GET",
        credentials: "include",
      }
    );

    const data = await response.json();

    if (!response.ok) {
      console.error(data);
      return [];
    }

    return data?.playlists as Playlist[];
  } catch (error) {
    console.error(error);
    return [];
  }
}
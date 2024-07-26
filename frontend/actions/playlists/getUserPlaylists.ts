"use server";

import { cookies } from "next/headers";

import { API_URL } from "@/api/http";
import { Playlist } from "@/types/types";

const getUserPlaylists = async (): Promise<Playlist[]> => {
  try {
    const xsrf = cookies().get(".AspNetCore.Xsrf")?.value ?? "";
    const response = await fetch(`${API_URL}/playlists/`, {
      headers: {
        "x-xsrf": xsrf,
        Cookie: cookies().toString(),
      },
      method: "GET",
      credentials: "include",
    });

    const data = await response.json();

    if (!response.ok) {
      throw new Error(data);
    }

    return data.playlists as Playlist[];
  } catch (error) {
    console.log(error);
    return [];
  }
}

export default getUserPlaylists;

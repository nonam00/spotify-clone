import {cookies} from "next/headers";

import {API_URL} from "@/api/http";
import {Playlist} from "@/types/types";
import getUserPlaylists from "./getUserPlaylists";

const getUserPlaylistsByQuantity = async (
  quantity: number
): Promise<Playlist[]> => {
  if (quantity <= 0) {
    return await getUserPlaylists();
  }
  try {
    const cookieStore = await cookies()
    const xsrf = cookieStore.get(".AspNetCore.Xsrf")?.value ?? "";
    const response = await fetch(
      `${API_URL}/playlists/certain/${quantity}`,
      {
        headers: {
          "x-xsrf-token": xsrf,
          Cookie: cookieStore.toString(),
        },
        method: "GET",
        credentials: "include",
      }
    );

    const data = await response.json();

    if (!response.ok) {
      throw new Error(data);
    }

    return data?.playlists as Playlist[];
  } catch (error) {
    console.log(error);
    return [];
  }
}

export default getUserPlaylistsByQuantity;

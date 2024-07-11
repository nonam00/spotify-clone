import { useState, useEffect } from "react";

import { Song } from "@/types/types";
import { API_URL } from "@/api/http";

const useLoadSongUrl = (song: Song) => {
  const [href, setHref] = useState<string | null>(null);
  useEffect(() => {
    if (!song) {
      return;
    }
    const getUrl = async() => {
      await fetch(`${API_URL}files/song/${song.songPath}`, {
        method: 'GET',
        credentials: "include"
      })
        .then((response) => response.text())
        .then((response) => setHref(response))
        .catch((error: Error) => {
          console.log(error.message)
        });
      }
    getUrl();
  }, [song]);

  return href;
}

export default useLoadSongUrl;

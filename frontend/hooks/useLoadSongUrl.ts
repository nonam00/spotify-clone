import { useState, useEffect } from "react";
import Cookies from "js-cookie";

import { Song } from "@/types/types";

const useLoadSongUrl = (song: Song) => {
  const [href, setHref] = useState<string | null>(null);
  useEffect(() => {
    if (!song) {
      return;
    }
    const getUrl = async() => {
      await fetch(`https://localhost:7025/1/files/song/${song.songPath}`, {
        method: 'GET',
        credentials: "include"
      })
        .then((response) => response.blob())
        .then((response) => {
          setHref(URL.createObjectURL(response));
        })
        .catch((error: Error) => {
          console.log(error.message)
        });
      }
    getUrl();
  }, [song]);

  return href;
}

export default useLoadSongUrl;
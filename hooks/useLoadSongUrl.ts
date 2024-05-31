import { useState, useEffect } from "react";

import $api from "@/api/http";
import { Song } from "@/types/types";

const useLoadSongUrl = (song: Song) => {
  const [href, setHref] = useState<string | null>(null);
  useEffect(() => {
    if (!song) {
      return;
    }
    const getUrl = async() => {
      try {
        await $api.get(`/files/get/song/${song.songPath}`, {
          responseType: "blob"
        })
        .then(response => {
          setHref(URL.createObjectURL(response.data));
        });
      } catch(error) {
        console.log(error);
      }
    }

    getUrl();
  }, [song]);

  return href;
}

export default useLoadSongUrl;
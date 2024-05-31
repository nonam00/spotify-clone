import $api from "@/api/http";

import { Song } from "@/types/types";
import { useEffect, useState } from "react";

const useLoadImage = (song: Song) => {
  const [href, setHref] = useState<string | null>(null);
  const path = song.imagePath;
  useEffect(() => {
    if (!song) {
      return;
    }
    const getUrl = async() => {
      try {
        await $api.get(`/files/get/image/${path}`, {
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
  }, [song, path]);

  return href;
};

export default useLoadImage;

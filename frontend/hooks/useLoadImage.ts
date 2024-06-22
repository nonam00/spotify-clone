import $api from "@/api/http";

import { Playlist, Song } from "@/types/types";
import { useEffect, useState } from "react";

const useLoadImage = (item: Song | Playlist | undefined) => {
  const [href, setHref] = useState<string | null>(null);
  const path = item? item.imagePath : null;
  
  useEffect(() => {
    if (!item || !item.imagePath) {
      return;
    }
    const getUrl = async() => {
      try {
        await $api.get(`/files/image/${path}`, {
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
  }, [item, path]);

  return href;
};

export default useLoadImage;

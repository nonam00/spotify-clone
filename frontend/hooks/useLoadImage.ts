import { useEffect, useState } from "react";

import $api from "@/api/http";
import { Playlist, Song } from "@/types/types";

const useLoadImage = (item: Song | Playlist | undefined) => {
  const [href, setHref] = useState<string | null>(null);
  const path = item? item.imagePath : null;

  useEffect(() => {
    if (!item || !item.imagePath) {
      setHref(null);
      return;
    }
    const getUrl = async() => {
      await $api.get(`/files/image/${path}`, {
        responseType: "blob"
      })
        .then(response => {
          setHref(URL.createObjectURL(response.data));
        })
        .catch(error => {
          setHref(null);
          console.log(error);
        });
    }

    getUrl();
  }, [item, path]);

  return href;
};

export default useLoadImage;

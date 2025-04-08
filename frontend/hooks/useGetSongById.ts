import {useState, useMemo, useLayoutEffect} from "react"

import { Song } from "@/types/types";
import { CLIENT_API_URL } from "@/api/http";

const useGetSongById = (id: string) => {
  const [isLoading, setIsLoading] = useState<boolean>(false);
  const [song, setSong] = useState<Song | undefined>(undefined);

  useLayoutEffect(() => {
    if (!id) {
      return;
    }
    const abortController = new AbortController();

    setIsLoading(true);

    fetch(`${CLIENT_API_URL}/songs/${id}`, {
      signal: abortController.signal,
    })
      .then(res => res.json())
      .then(setSong)

    setIsLoading(false);
    return () => {
      abortController.abort();
    }
  }, [id]);

  return useMemo(() => ({
    isLoading,
    song
  }), [isLoading, song]);
};

export default useGetSongById;

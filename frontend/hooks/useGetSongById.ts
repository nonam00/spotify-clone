import { useEffect, useState, useMemo } from "react"

import { Song } from "@/types/types";
import { API_URL } from "@/api/http";

const useGetSongById = (id?: string) => {
  const [isLoading, setIsLoading] = useState<boolean>(false);
  const [song, setSong] = useState<Song | undefined>(undefined);

  useEffect(() => {
    if (!id) {
      return;
    }

    setIsLoading(true);

    const fetchSong = async () => {
      const response = await fetch(`${API_URL}/songs/${id}`);

      if (response.ok) {
        const data = await response.json();
        setSong(data as Song);
      }

      setIsLoading(false);
    }

    fetchSong();
  }, [id]);

  return useMemo(() => ({
    isLoading,
    song
  }), [isLoading, song]);
};

export default useGetSongById;

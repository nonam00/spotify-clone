import { useEffect, useState, useMemo } from "react"
import { AxiosError } from "axios";

import $api from '@/api/http';
import { Song } from "@/types/types";

const useGetSongById = (id?: string) => {
  const [isLoading, setIsLoading] = useState<boolean>(false);
  const [song, setSong] = useState<Song | undefined>(undefined);

  useEffect(() => {
    if (!id) {
      return;
    }

    setIsLoading(true);

    const fetchSong = async () => {
      await $api.get<Song>(`/songs/${id}`)
        .then((response) => {
          setSong(response.data as Song);
          setIsLoading(false);
        })
        .catch((error: AxiosError) => {
          setIsLoading(false);
          console.log(error.response?.data);
        });
    }
    
    fetchSong();
  }, [id]);

  return useMemo(() => ({
    isLoading,
    song
  }), [isLoading, song]);
};

export default useGetSongById;

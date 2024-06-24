import $api from '@/api/http';
import { useEffect, useState, useMemo } from "react"
import toast from "react-hot-toast";

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
      let data;
      try {
        const response = await $api.get<Song>(`/songs/${id}`);
        data = response.data;
      } catch(error: any) {
        setIsLoading(false);
        return toast.error(error.message);
      }
      setSong(data as Song);
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

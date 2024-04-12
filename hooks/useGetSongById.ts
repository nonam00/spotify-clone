import { useSessionContext } from "@supabase/auth-helpers-react";
import { useEffect, useState, useMemo } from "react"
import toast from "react-hot-toast";

import { Song } from "@/types/types";

const useGetSongById = (id?: string) => {
  const [isLoading, setIsLoading] = useState<boolean>(false);
  const [song, setSong] = useState<Song | undefined>(undefined);
  // TODO: replace with own API
  const { supabaseClient } = useSessionContext();

  useEffect(() => {
    if (!id) {
      return;
    }

    setIsLoading(true);

    const fetchSong = async () => {
      const { data, error} = await supabaseClient
        .from('songs')
        .select('*')
        .eq('id', id)
        .single();

        if (error) {
          setIsLoading(false);
          return toast.error(error.message);
        }

        setSong(data as Song);
        setIsLoading(false);
    }

    fetchSong();
  }, [id, supabaseClient]);

  return useMemo(() => ({
    isLoading,
    song
  }), [isLoading, song]);
};

export default useGetSongById;
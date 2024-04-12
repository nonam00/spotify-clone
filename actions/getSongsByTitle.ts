import { Song } from "@/types/types";
import { createServerComponentClient } from "@supabase/auth-helpers-nextjs";
import { cookies } from "next/headers";
import getSongs from "./getSongs";


const getSongsByTitle= async (title: string) => {
  // TODO: replace with own API
  const supabase = createServerComponentClient({
    cookies: cookies
  });

  if (!title) {
    const allSongs = await getSongs();
    return allSongs;
  }
  
  const {
    data: sessionData,
    error: sessionError
  } = await supabase.auth.getUser();

  if (sessionError) {
    console.log(sessionError.message);
    return [];
  }

  const { data, error } = await supabase
    .from('songs')
    .select('*')
    .ilike('title', `%${title}%`)
    .eq('user_id', sessionData.user.id)
    .order('created_at', { ascending: false });

  if (error) {
    console.log(error.message);
  }

  return (data as any) || [];
};

export default getSongsByTitle;
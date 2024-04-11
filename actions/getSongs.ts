import { Song } from "@/types";
import { createServerComponentClient } from "@supabase/auth-helpers-nextjs";

import { headers, cookies } from "next/headers";

const getSongs = async (): Promise<Song[]> => {
  const supabase = createServerComponentClient({
    cookies: cookies
  });

  // TODO: replace with own API
  const { data, error } = await supabase
    .from('songs')
    .select('*')
    .order('created_at', { ascending: false });

  if (error) {
    console.log(error);
  }

  return (data as any) || [];
}

export default getSongs;
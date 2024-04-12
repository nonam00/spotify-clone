import { Song } from "@/types/types";
import { createServerComponentClient } from "@supabase/auth-helpers-nextjs";
import { cookies } from "next/headers";


const getSongsByUserId = async () => {
  // TODO: replace with own API
  const supabase = createServerComponentClient({
    cookies: cookies
  });

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
    .eq('user_id', sessionData.user.id)
    .order('created_at', { ascending: false });

  if (error) {
    console.log(error.message);
  }

  return (data as any) || [];
};

export default getSongsByUserId;
"use client";

import usePlayer from "@/hooks/usePlayer";
import useGetSongById from "@/hooks/useGetSongById";
//import useLoadSongUrl from "@/hooks/useLoadSongUrl";

import PlayerContent from "./PlayerContent";
import { API_URL } from "@/api/http";

const Player = () => {
  const activeSongId = usePlayer(s => s.activeId);
  const { song } = useGetSongById(activeSongId);
  //const songUrl = useLoadSongUrl(song!); 

  if(!song || !activeSongId) {
    return null;
  }

  const songUrl = `${API_URL}/files/song/${song.songPath}`;
  
  return (
    <div
      className="
        fixed
        bottom-0
        bg-black
        w-full
        py-2
        h-[80px]
        px-4
      "
    >
      <PlayerContent
        key={songUrl} // to completely destructorize an object
        song={song}
        songUrl={songUrl}
      />
    </div>
  );
}
 
export default Player;

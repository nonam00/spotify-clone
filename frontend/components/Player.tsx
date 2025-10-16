"use client";

import usePlayerStorage from "@/hooks/usePlayerStorage";
import useGetSongById from "@/hooks/useGetSongById";

import PlayerContent from "./PlayerContent";

import {CLIENT_FILES_URL} from "@/helpers/api";

const Player = () => {
  const activeSongId = usePlayerStorage(s => s.activeId);
  const { song } = useGetSongById(activeSongId!);

  if(!song || !activeSongId) {
    return null;
  }

  const songUrl = `${CLIENT_FILES_URL}/download-url?type=audio&file_id=${song.songPath}`;
  
  return (
    <div className="fixed bottom-0 bg-black w-full py-2 h-15">
      <PlayerContent
        key={songUrl}
        song={song}
        songUrl={songUrl}
      />
    </div>
  );
}
 
export default Player;

"use client";

import { useRouter } from "next/navigation";
import { useEffect } from "react";
import { FaPlay } from "react-icons/fa";

import { Song } from "@/types/types"; 

import MediaItem from "@/components/MediaItem";
import LikeButton from "@/components/LikeButton";
import RemoveButton from "@/components/RemoveButton";

import { useUser } from "@/hooks/useUser";
import useOnPlay from "@/hooks/useOnPlay";

interface PlaylistContentProps {
  id: string,
  songs: Song[];
}

const PlaylistContent: React.FC<PlaylistContentProps> = ({
  id,
  songs
}) => {
  const router = useRouter();
  const { isLoading, isAuth } = useUser();

  const onPlay = useOnPlay(songs);
  
  useEffect(() => {
    if (!isLoading && !isAuth) {
      console.log("replace");
      router.replace('/');
    }
  }, [isLoading, isAuth, router]);

  return (
    <div>
      <div className="flex flex-row align-middle gap-y-5 items-start p-6">
        <div
          className="
            transition
            rounded-full
            flex
            items-center
            justify-center
            bg-green-500
            p-4
            drop-shadow-md
            right-5
            hover:scale-110
        "
      >
        <FaPlay className="text-black"/>
      </div>
      </div>
      <div className="flex flex-col align-middle gap-y-2 w-full p-6">
        {songs.length === 0?
          <div className="
            flex
            flex-col
            text-neutral-400
            items-center
            md:items-start
          ">
            There are no songs in this playlist.
          </div>
          : songs.map((song) => (

          <div
            key={song.id}
            className="flex items-center gap-4 w-full"
          >
            <div className="flex-1">
              <MediaItem
                onClick={(id: string) => onPlay(id)}
                data={song}
              />
            </div>
            <div className="flex flex-row gap-x-6">
              <RemoveButton playlistId={id} songId={song.id}/>
              <LikeButton songId={song.id} />
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}
 
export default PlaylistContent;

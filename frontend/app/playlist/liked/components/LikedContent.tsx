"use client";

import { useRouter } from "next/navigation";
import {memo, useLayoutEffect} from "react";

import { Song } from "@/types/types";
import MediaItem from "@/components/MediaItem";
import LikeButton from "@/components/LikeButton";
import { useUser } from "@/hooks/useUser";
import useOnPlay from "@/hooks/useOnPlay";

const LikedContent = memo(function LikedContent({
  songs
}: {
  songs: Song[]
}) {
  const router = useRouter();
  const { isLoading, isAuth } = useUser();

  const onPlay = useOnPlay(songs);

  useLayoutEffect(() => {
    if (!isLoading && !isAuth) {
      router.replace('/');
    }
  }, [isLoading, isAuth, router]);

  if (songs.length === 0) {
    return (
      <div className="flex flex-col gap-y-2 px-6 text-neutral-400">
        No liked songs.
      </div>
    )
  }

  return (
    <div className="flex flex-col gap-y-2 w-full p-6">
      {songs.map((song) => (
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
          <LikeButton songId={song.id} defaultValue={true}/>
        </div>
      ))}
    </div>
  );
});
 
export default LikedContent;

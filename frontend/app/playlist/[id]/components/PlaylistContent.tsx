"use client";

import { useRouter } from "next/navigation";
import {startTransition, useState} from "react";
import { FaPlay } from "react-icons/fa";
import { AiOutlinePlusCircle } from "react-icons/ai";

import { Song } from "@/types/types";

import DeletePlaylistButton from "./DeletePlaylistButton";
import RemoveButton from "./RemoveButton";

import useOnPlay from "@/hooks/useOnPlay";
import SongListItem from "@/components/SongListItem";

const PlaylistContent = ({
  id,
  initialSongs
}: {
  id: string;
  initialSongs: Song[];
}) => {
  const router = useRouter();
  const [songs, setSongs] = useState<Song[]>(initialSongs);
  const onPlay = useOnPlay(songs);

  const onPlayClick = () => {
    if (songs.length === 0) return;
    onPlay(songs[0].id);
  }

  const onRemoveClick = (songId: string) => {
    startTransition(() =>{
      setSongs(prevSongs => prevSongs.filter(song => song.id !== songId));
    })
  }

  return (
    <div>
      <div className="flex flex-row align-middle gap-y-2 items-center justify-start px-6 py-3">
        <button
          onClick={onPlayClick}
          className="
            flex items-center justify-start p-5 right-5
            rounded-full bg-green-500 drop-shadow-md hover:scale-110 transition
          "
          aria-label="Play playlist"
        >
          <FaPlay className="text-black cursor-pointer" size="20" />
        </button>
        <button
          onClick={() => { router.push(`/playlist/${id}/add?searchString=&type=all`) }}
          className="flex flex-end mx-5 rounded-full hover:scale-105 cursor-pointer"
        >
          <AiOutlinePlusCircle className="text-neutral-400" size="35" />
        </button>
        <DeletePlaylistButton playlistId={id} />
      </div>
      <div className="h-0.5 bg-neutral-800/40 w-full" />
      <div className="flex flex-col align-middle gap-y-5 w-full p-6">
        {songs.length === 0
          ?
            <div className="flex flex-col text-neutral-400 items-center md:items-start">
              There are no songs in this playlist.
            </div>
          : songs.map((song) => (
              <SongListItem
                key={song.id}
                song={song}
                onClickCallback={onPlay}
              >
                <RemoveButton
                  playlistId={id}
                  songId={song.id}
                  callback={onRemoveClick}
                />
              </SongListItem>
            ))
        }
      </div>
    </div>
  );
}

export default PlaylistContent;

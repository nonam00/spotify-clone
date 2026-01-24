"use client";

import Image from "next/image";
import {useCallback} from "react";
import { useShallow } from "zustand/shallow";

import { CLIENT_FILES_URL } from "@/shared/config/api";
import type { Playlist } from "@/entities/playlist";
import { useUpdatePlaylistModalStore } from "../model";

const PlaylistImage = ({ playlist }: { playlist: Playlist }) => {
  const { setPlaylist, onOpen } = useUpdatePlaylistModalStore(
    useShallow((s) => ({
      setPlaylist: s.setPlaylist,
      onOpen: s.onOpen
    })),
  );

  const onClickCallback = useCallback(() => {
    setPlaylist(playlist);
    onOpen();
  }, [onOpen, playlist, setPlaylist]);

  return (
    <Image
      fill
      alt={playlist.title}
      className="object-cover rounded-xl hover:opacity-75 transition cursor-pointer"
      src={
        playlist.imagePath
          ? `${CLIENT_FILES_URL}/download-url?type=image&file_id=${playlist.imagePath}`
          : "/images/playlist.webp"
      }
      unoptimized
      loading="lazy"
      onClick={onClickCallback}
    />
  );
};

export default PlaylistImage;


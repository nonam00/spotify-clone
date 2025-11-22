"use client";

import Image from "next/image";
import { useShallow } from "zustand/shallow";

import { CLIENT_FILES_URL } from "@/shared/config/api";
import type { Playlist } from "@/entities/playlist/model";
import { useUpdatePlaylistModalStore } from "../model";

const PlaylistImage = ({ playlist }: { playlist: Playlist }) => {
  const [setPlaylist, onOpen] = useUpdatePlaylistModalStore(
    useShallow((s) => [s.setPlaylist, s.onOpen])
  );
  const onClick = () => {
    setPlaylist(playlist);
    onOpen();
  };
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
      onClick={onClick}
    />
  );
};

export default PlaylistImage;


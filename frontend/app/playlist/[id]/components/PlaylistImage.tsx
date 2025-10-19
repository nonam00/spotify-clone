"use client";

import Image from "next/image";
import { useShallow } from "zustand/shallow";

import { Playlist } from "@/types/types";
import usePlaylistModal from "@/hooks/usePlaylistModal";

import {CLIENT_API_URL, CLIENT_FILES_URL} from "@/helpers/api";

const PlaylistImage = ({
  playlist
}: {
  playlist: Playlist
}) => {
  const [setPlaylist, onOpen] = usePlaylistModal(useShallow(s => [s.setPlaylist, s.onOpen]));
  const onClick = () => {
    setPlaylist(playlist);
    onOpen();
  }
  return (
    <Image
      fill
      alt={playlist.title}
      className="object-cover rounded-xl hover:opacity-75 transition"
      src={playlist.imagePath
        ? `${CLIENT_FILES_URL}/download-url?type=image&file_id=${playlist.imagePath}`
        : "/images/playlist.webp"
      }
      unoptimized
      loading="lazy"
      onClick={onClick}
    />
  )
}

export default PlaylistImage;

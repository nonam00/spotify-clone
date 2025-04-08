"use client";

import Image from "next/image";
import { useShallow } from "zustand/shallow";

import { Playlist } from "@/types/types";
import usePlaylistModal from "@/hooks/usePlaylistModal";
import { CLIENT_API_URL } from "@/api/http";

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
      alt="Playlist"
      className="object-cover rounded-xl hover:opacity-75 transition"
      src={playlist.imagePath
        ? `${CLIENT_API_URL}/files/image/${playlist.imagePath}`
        : "/images/playlist.webp"
      }
      unoptimized
      onClick={onClick}
    />
  )
}

export default PlaylistImage;

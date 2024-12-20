"use client";

import Image from "next/image";

import { Playlist } from "@/types/types";
import usePlaylistModal from "@/hooks/usePlaylistModal";
import { API_URL } from "@/api/http";
import { useShallow } from "zustand/shallow";

interface PlaylistImageProps {
  playlist: Playlist
}

const PlaylistImage: React.FC<PlaylistImageProps> = ({
  playlist
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
        ? `${API_URL}/files/image/${playlist.imagePath}`
        : "/images/playlist.webp"
      }
      unoptimized
      onClick={onClick}
    />
  )
}

export default PlaylistImage;

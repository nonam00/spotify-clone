"use client"

import Image from "next/image";

import useLoadImage from "@/hooks/useLoadImage";
import { Playlist } from "@/types/types";
import usePlaylistModal from "@/hooks/usePlaylistModal";

interface PlaylistImageProps {
  playlist: Playlist
}

const PlaylistImage: React.FC<PlaylistImageProps> = ({
  playlist
}) => {
  const { setPlaylist, onOpen } = usePlaylistModal();
  const href = useLoadImage(playlist);
  const onClick = () => {
    setPlaylist(playlist);
    onOpen();
  }
  return (
    <Image
      fill
      alt="Playlist"
      className="object-cover rounded-xl hover:opacity-75 transition"
      src={href ?? "/images/playlist.webp"}
      onClick={onClick}
    />
  )
}

export default PlaylistImage;

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
  const playlistModal = usePlaylistModal();
  const href = useLoadImage(playlist);
  const onClick = () => {
    playlistModal.setPlaylist(playlist);
    playlistModal.onOpen();
    console.log(playlistModal.id);
  }
  return (
    <Image
      fill
      alt="Playlist"
      className="object-cover rounded-xl"
      src={href ?? "/images/liked.png"}
      onClick={onClick}
    />
  )
}

export default PlaylistImage;

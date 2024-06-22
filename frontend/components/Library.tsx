"use client";

import { AiOutlinePlus } from "react-icons/ai";
import { TbPlaylist } from "react-icons/tb";

import { Playlist } from "@/types/types";

import useAuthModal from "@/hooks/useAuthModal";
import useUploadModal from "@/hooks/useUploadModal";
import { useUser } from "@/hooks/useUser";

import MediaItem from "./MediaItem";
import { useRouter } from "next/navigation";
import $api from "@/api/http";

interface LibraryProps {
  playlists: Playlist[];
}

const Library: React.FC<LibraryProps> = ({
  playlists
}) => {
  const authModal = useAuthModal();
  const uploadModal = useUploadModal();
  const { isAuth } = useUser();
  const router = useRouter();

  const onUploadClick = () => {
    if (!isAuth) {
      return authModal.onOpen();
    }

    return uploadModal.onOpen();
  };

  const onPlaylistClick = () => {
    if (!isAuth) {
      return authModal.onOpen();
    }
    $api.post("/playlists/")
      .then(response => {
        if (response.status >= 200 && response.status < 400) {
          router.push(`/playlist?id=${response.data}`)
          router.refresh();
        }
      })
  }

  return (
    <div className="flex flex-col">
      <div
        className="
          flex
          items-center
          justify-between
          px-5
          pt-4
        "
      >
        <div
          className="
            inline-flex
            items-center
            gap-x-2
          "
        >
          <TbPlaylist className="text-neutral-400"size={26} />
          <p
            className="
              text-neutral-400
              font-medium
              text-md
            "
          >
            Your Library
          </p>
        </div>
        <AiOutlinePlus
          onClick={onUploadClick}
          size={20}
          className="
            text-neutral-400
            cursor-pointer
            hover:text-white
            transition
          "
        />
      </div>
      <button
        onClick={onPlaylistClick}
        className="
          text-neutral-400
          font-medium
          text-md
        "
      >
        Create Playlist 
      </button>

      <div className="
        flex
        flex-col
        gap-y-5
        mt-4
        px-3
      ">
        {playlists.map((playlist) => (
          <MediaItem
            onClick={(id: string) => router.push(`/playlist?id=${id}`)}
            key={playlist.id}
            data={playlist}
          />
        ))}
      </div>
    </div>
  );
}
export default Library;

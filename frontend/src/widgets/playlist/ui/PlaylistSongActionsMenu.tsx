"use client";

import { useTransition } from "react";
import toast from "react-hot-toast";
import { HiOutlineTrash } from "react-icons/hi";

import { OptionsMenu } from "@/shared/ui";
import { removeSongFromPlaylist } from "@/entities/playlist/api";

type SongActionsMenuProps = {
  playlistId: string;
  songId: string;
  callback?: (songId: string) => void;
  disabled?: boolean;
};

const PlaylistSongActionsMenu = ({
  playlistId,
  songId,
  callback,
  disabled,
}: SongActionsMenuProps) => {
  const [isPending, startTransition] = useTransition();

  const handleRemove = async () => {
    startTransition(async () => {
      const success = await removeSongFromPlaylist(playlistId, songId);
      if (success) {
        callback?.(songId);
        toast.success("Song removed from playlist.");
      } else {
        toast.error("Failed to remove song.");
      }
    });
  };

  return (
    <OptionsMenu
      buttonAriaLabel="Actions with song"
      disabled={disabled || isPending}
      options={[
        {
          id: "remove-from-playlist",
          label: "Remove from playlist",
          icon: <HiOutlineTrash />,
          isDestructive: true,
          disabled: disabled || isPending,
          onSelect: handleRemove,
        },
      ]}
    />
  );
};

export default PlaylistSongActionsMenu;


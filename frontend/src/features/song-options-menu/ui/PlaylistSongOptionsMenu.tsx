"use client";

import { useCallback, useState } from "react";
import { HiOutlineTrash, HiOutlinePlus } from "react-icons/hi";
import toast from "react-hot-toast";

import { OptionsMenu } from "@/shared/ui";
import { removeSongFromPlaylist } from "@/entities/playlist";
import { useAddToPlaylistsOption } from "@/features/song-options-menu/lib";

type PlaylistSongOptionsMenuProps = {
  playlistId: string;
  songId: string;
  songTitle?: string;
  disabled?: boolean;
  removeCallback?(songId: string): void;
  triggerContent?: React.ReactNode;
  className?: string;
};

export function PlaylistSongOptionsMenu({
  playlistId,
  songId,
  songTitle,
  disabled = false,
  removeCallback,
  triggerContent,
  className,
}: PlaylistSongOptionsMenuProps) {
  const {
    addToPlaylistSubmenu,
    loadPlaylistsCallback,
    isAdding
  } = useAddToPlaylistsOption(songId);

  const [isRemoving, setIsRemoving] = useState(false);

  const handleRemove = useCallback(async () => {
    setIsRemoving(true);
    try {
      const success = await removeSongFromPlaylist(playlistId, songId);
      if (success) {
        removeCallback?.(songId);
        toast.success("Song removed from playlist");
      } else {
        toast.error("Failed to remove song");
      }
    } catch (error) {
      console.error("Failed to remove song from playlist", error);
      toast.error("Failed to remove song");
    } finally {
      setIsRemoving(false);
    }
  }, [playlistId, songId, removeCallback]);

  const options = [
    {
      id: "add-to-playlist",
      label: "Add to other playlist",
      icon: <HiOutlinePlus />,
      disabled: disabled || isAdding,
      submenu: addToPlaylistSubmenu,
      onSelect: () => {}
    },
    {
      id: "remove-from-playlist",
      label: "Remove from playlist",
      icon: <HiOutlineTrash />,
      isDestructive: true,
      disabled: disabled || isRemoving,
      onSelect: handleRemove,
    },
  ];

  return (
    <OptionsMenu
      options={options}
      buttonAriaLabel={`Actions for ${songTitle || "song"}`}
      disabled={disabled || isRemoving || isAdding}
      triggerContent={triggerContent}
      className={className}
      onOpen={loadPlaylistsCallback}
      side="left"
      align="start"
    />
  );
}
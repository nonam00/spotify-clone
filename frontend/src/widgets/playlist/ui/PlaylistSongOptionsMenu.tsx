"use client";

import { HiOutlineTrash, HiOutlinePlus } from "react-icons/hi";
import { OptionsMenu } from "@/shared/ui";
import {
  getPlaylistsWithoutSong,
  addSongToPlaylist,
  removeSongFromPlaylist,
  type Playlist,
} from "@/entities/playlist";
import {useCallback, useMemo, useState} from "react";
import toast from "react-hot-toast";
import {CLIENT_FILES_URL} from "@/shared/config/api";

type PlaylistSongOptionsMenuProps = {
  playlistId: string;
  songId: string;
  songTitle?: string;
  disabled?: boolean;
  removeCallback?: (songId: string) => void;
  triggerContent?: React.ReactNode;
  className?: string;
};

const PlaylistSongOptionsMenu = ({
  playlistId,
  songId,
  songTitle,
  disabled = false,
  removeCallback,
  triggerContent,
  className,
}: PlaylistSongOptionsMenuProps) => {
  const [isRemoving, setIsRemoving] = useState(false);
  const [isAdding, setIsAdding] = useState(false);
  const [playlists, setPlaylists] = useState<Playlist[]>([]);
  const [isLoadingPlaylists, setIsLoadingPlaylists] = useState(false);

  const loadPlaylists = useCallback(async () => {
    setIsLoadingPlaylists(true);
    try {
      const receivedPlaylists = await getPlaylistsWithoutSong(songId);
      setPlaylists(receivedPlaylists);
    } catch (error) {
      console.error("Failed to load playlists", error);
      toast.error("Failed to load playlists");
    } finally {
      setIsLoadingPlaylists(false);
    }
  }, [songId]);

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

  const handleAddToPlaylist = useCallback(
    async (targetPlaylistId: string, playlistName: string) => {
      setIsAdding(true);
      try {
        const success = await addSongToPlaylist(targetPlaylistId, songId);

        if (success) {
          toast.success(`Added to "${playlistName}"`);
          setPlaylists((prev) =>
            prev.filter((playlist) => playlist.id !== targetPlaylistId)
          );
        } else {
          toast.error("Failed to add to playlist");
        }
      } catch (error) {
        console.error("Failed to add song to playlist", error);
        toast.error("Failed to add to playlist");
      } finally {
        setIsAdding(false);
      }
    },
    [songId]
  );

  const addToPlaylistSubmenu = useMemo(() => {
    if (isLoadingPlaylists) {
      return [
        {
          id: "loading",
          label: "Loading playlists...",
          disabled: true,
          onSelect: () => {},
        },
      ];
    }

    if (playlists.length === 0) {
      return [
        {
          id: "no-playlists",
          label: "No other playlists",
          disabled: true,
          onSelect: () => {},
        },
      ];
    }

    return playlists.map((playlist) => ({
      id: `add-to-${playlist.id}`,
      label: playlist.title,
      imageUrl: `${CLIENT_FILES_URL}/download-url?type=image&file_id=${playlist.imagePath}`,
      disabled: isAdding,
      onSelect: () => handleAddToPlaylist(playlist.id, playlist.title),
    }));
  }, [playlists, isLoadingPlaylists, isAdding, handleAddToPlaylist]);

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
      onOpen={loadPlaylists}
      side="left"
      align="start"
    />
  );
};

export default PlaylistSongOptionsMenu;
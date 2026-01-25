"use client";

import {useCallback, useMemo, useState} from "react";
import { HiOutlinePlus } from "react-icons/hi";
import toast from "react-hot-toast";

import { OptionsMenu } from "@/shared/ui";
import {
  getPlaylistsWithoutSong,
  addSongToPlaylist,
  type Playlist,
} from "@/entities/playlist";
import {CLIENT_FILES_URL} from "@/shared/config/api";

type SongOptionsMenuProps = {
  songId: string;
  disabled?: boolean;
  triggerContent?: React.ReactNode;
  className?: string;
};

const SongOptionsMenu = ({
  songId,
  disabled = false,
  triggerContent,
  className,
}: SongOptionsMenuProps) => {
  const [playlists, setPlaylists] = useState<Playlist[]>([]);
  const [isLoadingPlaylists, setIsLoadingPlaylists] = useState(false);
  const [isAdding, setIsAdding] = useState(false);

  const loadPlaylists = useCallback(async () => {
    setIsLoadingPlaylists(true);
    try {
      const data = await getPlaylistsWithoutSong(songId);
      setPlaylists(data);
    } catch (error) {
      console.error("Failed to load playlists", error);
      toast.error("Failed to load playlists");
    } finally {
      setIsLoadingPlaylists(false);
    }
  }, [songId]);

  const handleAddToPlaylist = useCallback(
    async (playlistId: string, playlistName: string) => {
      setIsAdding(true);
      try {
        const success = await addSongToPlaylist(playlistId, songId,);

        if (success) {
          toast.success(`Added to "${playlistName}"`);
          setPlaylists((prev) =>
            prev.filter((playlist) => playlist.id !== playlistId)
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
          label: "No available playlists",
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
      label: "Add to playlist",
      icon: <HiOutlinePlus />,
      disabled: disabled || isAdding,
      submenu: addToPlaylistSubmenu,
      onSelect: () => {}
    },
  ];

  return (
    <OptionsMenu
      options={options}
      buttonAriaLabel={`Actions for song`}
      disabled={disabled || isAdding}
      triggerContent={triggerContent}
      className={className}
      onOpen={loadPlaylists}
      side="left"
      align="start"
    />
  );
};

export default SongOptionsMenu;
import { useCallback, useMemo, useState } from "react";
import toast from "react-hot-toast";

import { MenuOption } from "@/shared/ui";
import { CLIENT_FILES_URL } from "@/shared/config/api";
import { addSongToPlaylist, getPlaylistsWithoutSong, type Playlist } from "@/entities/playlist";

export function useAddToPlaylistsOption(songId: string): {
  addToPlaylistSubmenu: MenuOption[],
  loadPlaylistsCallback: () => void,
  isAdding: boolean,
} {
  const [playlists, setPlaylists] = useState<Playlist[]>([]);
  const [isLoadingPlaylists, setIsLoadingPlaylists] = useState(false);
  const [isAdding, setIsAdding] = useState(false);

  const loadPlaylistsCallback = useCallback(async () => {
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
      imageUrl: playlist.imagePath
        ? `${CLIENT_FILES_URL}/download-url?type=image&file_id=${playlist.imagePath}`
        : "/images/playlist.webp",
      disabled: isAdding,
      onSelect: () => handleAddToPlaylist(playlist.id, playlist.title),
    }));
  }, [playlists, isLoadingPlaylists, isAdding, handleAddToPlaylist]);

  return {
    addToPlaylistSubmenu,
    loadPlaylistsCallback,
    isAdding,
  };
}
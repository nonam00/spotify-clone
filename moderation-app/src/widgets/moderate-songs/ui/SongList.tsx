import { useEffect } from "react";

import { Box, Button } from "@/shared/ui";
import { useConfirmModalStore } from "@/features/confirm-modal";
import { useSongsStore } from "../model";
import SongModerationItem from "./SongModerationItem";
import {useShallow} from "zustand/react/shallow";

const SongList = () => {
  const { 
    songs, 
    isLoading, 
    error, 
    selectedSongs,
    fetchSongs,
    publishSelectedSongs,
    deleteSelectedSongs,
    selectAll,
    clearSelection 
  } = useSongsStore();

  const onOpen = useConfirmModalStore(useShallow((s) => s.onOpen));

  useEffect(() => {
    fetchSongs().catch(() => undefined);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const handlePublishSelected = () => {
    onOpen(
      "Publish selected songs?",
      `Are you sure, you want to publish ${selectedSongs.length} ${selectedSongs.length === 1 ? 'song' : 'songs'}?`
      + ` After publication ${selectedSongs.length === 1 ? 'it' : 'they'} will be available to all users.`,
      async () => {
        await publishSelectedSongs();
      }
    );
  };

  const handleDeleteSelected = () => {
    onOpen(
      "Delete selected songs?",
      `Are you sure, you want to delete ${selectedSongs.length} ${selectedSongs.length === 1 ? 'song' : 'songs'}?`
      + ` This action cannot be undone.`,
      async () => {
        await deleteSelectedSongs();
      }
    );
  };

  if (isLoading && songs.length === 0) {
    return (
      <Box className="p-12">
        <div className="flex flex-col items-center justify-center gap-4">
          <div className="animate-spin rounded-full h-12 w-12 border-4 border-green-500 border-t-transparent"></div>
          <p className="text-white text-lg font-medium">Loading songs...</p>
        </div>
      </Box>
    );
  }

  if (error) {
    return (
      <Box className="p-8">
        <div className="flex flex-col items-center gap-4">
          <div className="text-red-400 text-lg font-semibold">Error on loading</div>
          <p className="text-red-300 text-center">{error}</p>
          <Button 
            onClick={fetchSongs} 
            className="bg-green-500 hover:bg-green-600 text-white mt-2"
          >
            Try again
          </Button>
        </div>
      </Box>
    );
  }

  if (songs.length === 0) {
    return (
      <Box className="p-12">
        <div className="flex flex-col items-center justify-center gap-3">
          <div className="text-6xl mb-2">🎵</div>
          <p className="text-white text-xl font-semibold">There are no unpublished songs</p>
          <p className="text-neutral-400 text-sm font-medium">Wait for our users to upload new songs...</p>
        </div>
      </Box>
    );
  }

  return (
    <Box className="p-6">
      <div className="flex items-center justify-between mb-6 flex-wrap gap-4 pb-4 border-b border-neutral-700/30">
        <div className="flex items-center gap-x-3 flex-wrap">
          <Button
            onClick={selectAll}
            disabled={isLoading || songs.length === 0}
            className="bg-neutral-700 hover:bg-neutral-600 text-white px-5 py-2.5 text-sm font-medium disabled:opacity-50"
          >
            Select all
          </Button>
          <Button
            onClick={clearSelection}
            disabled={isLoading || selectedSongs.length === 0}
            className="bg-neutral-700 hover:bg-neutral-600 text-white px-5 py-2.5 text-sm font-medium disabled:opacity-50"
          >
            Cancel selection
          </Button>
          {selectedSongs.length > 0 && (
            <>
              <Button
                onClick={handlePublishSelected}
                disabled={isLoading}
                className="bg-green-500 hover:bg-green-600 text-white px-5 py-2.5 text-sm font-medium shadow-md"
              >
                Publish selected ({selectedSongs.length})
              </Button>
              <Button
                onClick={handleDeleteSelected}
                disabled={isLoading}
                className="bg-red-500 hover:bg-red-600 text-white px-5 py-2.5 text-sm font-medium shadow-md"
              >
                Delete selected ({selectedSongs.length})
              </Button>
            </>
          )}
        </div>
        <div className="flex items-center gap-2 px-4 py-2 rounded-lg bg-neutral-800/50 border border-neutral-700/30">
          <span className="text-neutral-300 text-sm font-medium">
            Total: <span className="text-white font-bold">{songs.length}</span>
          </span>
          <span className="text-neutral-500">•</span>
          <span className="text-neutral-300 text-sm font-medium">
            Selected: <span className="text-green-400 font-bold">{selectedSongs.length}</span>
          </span>
        </div>
      </div>
      {isLoading && songs.length > 0 && (
        <div className="mb-4 flex items-center justify-center py-2">
          <div className="animate-spin rounded-full h-5 w-5 border-2 border-green-500 border-t-transparent"></div>
          <p className="text-neutral-400 ml-3 text-sm font-medium">Updating...</p>
        </div>
      )}
      <div className="flex flex-col gap-y-4">
        {songs.map((song) => (
          <SongModerationItem key={song.id} song={song} />
        ))}
      </div>
    </Box>
  );
};

export default SongList;
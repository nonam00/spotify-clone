"use client";

import { startTransition, useState } from "react";
import { FaPlay } from "react-icons/fa";
import toast from "react-hot-toast";
import {
  DndContext,
  closestCenter,
  KeyboardSensor,
  PointerSensor,
  useSensor,
  useSensors,
  DragEndEvent,
} from "@dnd-kit/core";
import {
  arrayMove,
  SortableContext,
  sortableKeyboardCoordinates,
  verticalListSortingStrategy,
} from "@dnd-kit/sortable";

import { SortableSongListItem, type Song } from "@/entities/song";
import { reorderPlaylistSongs } from "@/entities/playlist";
import { useOnPlay } from "@/widgets/player";
import { PlaylistSongActionsMenu, PlaylistActionsMenu } from "@/widgets/playlist";

const PlaylistContent = ({
  id,
  initialSongs,
}: {
  id: string;
  initialSongs: Song[];
}) => {
  const [songs, setSongs] = useState<Song[]>(initialSongs);
  const onPlay = useOnPlay(songs);

  const sensors = useSensors(
    useSensor(PointerSensor, {
      activationConstraint: {
        distance: 8,
      },
    }),
    useSensor(KeyboardSensor, {
      coordinateGetter: sortableKeyboardCoordinates,
    })
  );

  const onPlayClick = () => {
    if (songs.length === 0) return;
    onPlay(songs[0].id);
  };

  const onRemoveClick = (songId: string) => {
    startTransition(() =>
      setSongs((prevSongs) => prevSongs.filter((song) => song.id !== songId))
    );
  };

  const handleDragEnd = async (event: DragEndEvent) => {
    const { active, over } = event;

    if (!(over && active.id !== over.id)) return;

    const oldIndex = songs.findIndex((song) => song.id === active.id);
    const newIndex = songs.findIndex((song) => song.id === over.id);

    const newSongs = arrayMove(songs, oldIndex, newIndex);
    setSongs(newSongs);

    const songIds = newSongs.map((song) => song.id).reverse();
    const success = await reorderPlaylistSongs(id, songIds);

    if (!success) {
      setSongs(songs);
      toast.error("Failed to reorder like-button");
    }
  };

  return (
    <div>
      <div className="flex flex-row align-middle gap-x-2 items-center justify-start px-6 py-3">
        <button
          onClick={onPlayClick}
          className="
            flex items-center justify-start p-5 right-5
            rounded-full bg-green-500 drop-shadow-md hover:scale-110 transition
          "
          aria-label="Play playlist"
        >
          <FaPlay className="text-black cursor-pointer" size="20" />
        </button>
        <PlaylistActionsMenu playlistId={id} />
      </div>
      <div className="h-0.5 bg-neutral-800/40 w-full" />
      <DndContext
        sensors={sensors}
        collisionDetection={closestCenter}
        onDragEnd={handleDragEnd}
      >
        <SortableContext
          items={songs.map((song) => song.id)}
          strategy={verticalListSortingStrategy}
        >
          <div className="flex flex-col align-middle gap-y-5 w-full p-6">
            {songs.length === 0 ? (
              <div className="flex flex-col text-neutral-400 items-center md:items-start">
                There are no songs in this playlist.
              </div>
            ) : (
              songs.map((song) => (
                <SortableSongListItem
                  key={song.id}
                  song={song}
                  onClickCallback={onPlay}
                >
                  <PlaylistSongActionsMenu
                    playlistId={id}
                    songId={song.id}
                    callback={onRemoveClick}
                  />
                </SortableSongListItem>
              ))
            )}
          </div>
        </SortableContext>
      </DndContext>
    </div>
  );
};

export default PlaylistContent;
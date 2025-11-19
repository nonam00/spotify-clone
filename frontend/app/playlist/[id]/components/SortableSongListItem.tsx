"use client";

import { useSortable } from "@dnd-kit/sortable";
import { CSS } from "@dnd-kit/utilities";

import { Song } from "@/types/types";
import SongListItem from "@/components/SongListItem";

const SortableSongListItem = ({
  song,
  onClickCallback,
  children
}: {
  song: Song;
  onClickCallback: (id: string) => void;
  children?: React.ReactNode;
}) => {
  const {
    attributes,
    listeners,
    setNodeRef,
    transform,
    transition,
    isDragging,
  } = useSortable({ id: song.id });

  const style = {
    transform: CSS.Transform.toString(transform),
    transition,
    opacity: isDragging ? 0.5 : 1,
  };

  return (
    <div
      ref={setNodeRef}
      style={style}
      {...attributes}
      {...listeners}
      className="cursor-grab active:cursor-grabbing touch-none"
      aria-label="Drag to change order of songs"
    >
      <SongListItem
        song={song}
        onClickCallback={onClickCallback}
      >
        {children}
      </SongListItem>
    </div>
  );
};

export default SortableSongListItem;


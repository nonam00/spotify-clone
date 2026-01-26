"use client";

import { HiOutlinePlus } from "react-icons/hi";
import { OptionsMenu } from "@/shared/ui";
import {useAddToPlaylistsOption} from "@/features/song-options-menu/lib";

type SongOptionsMenuProps = {
  songId: string;
  disabled?: boolean;
  triggerContent?: React.ReactNode;
  className?: string;
};

export function SongOptionsMenu({
  songId,
  disabled = false,
  triggerContent,
  className,
}: SongOptionsMenuProps) {
  const {
    addToPlaylistSubmenu,
    loadPlaylistsCallback,
    isAdding
  } = useAddToPlaylistsOption(songId);

  const options = [
    {
      id: "add-to-playlist",
      label: "Add to playlist",
      icon: <HiOutlinePlus />,
      disabled: disabled,
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
      onOpen={loadPlaylistsCallback}
      side="left"
      align="start"
    />
  );
}
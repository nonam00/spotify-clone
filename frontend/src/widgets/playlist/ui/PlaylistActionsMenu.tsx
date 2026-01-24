"use client";

import { useRouter } from "next/navigation";
import { useCallback, useTransition } from "react";
import { useShallow } from "zustand/shallow";
import { AiOutlinePlusCircle } from "react-icons/ai";
import { FaTrash } from "react-icons/fa";
import toast from "react-hot-toast";

import { OptionsMenu } from "@/shared/ui";
import { deletePlaylist } from "@/entities/playlist";
import { useConfirmModalStore } from "@/features/confirm-modal";

type PlaylistActionsMenuProps = {
  playlistId: string;
  disabled?: boolean;
};

const PlaylistActionsMenu = ({
  playlistId,
  disabled,
}: PlaylistActionsMenuProps) => {
  const router = useRouter();
  const [isPending, startTransition] = useTransition();

  const { onOpen, setAction, setDescription } = useConfirmModalStore(
    useShallow((s) => ({
      onOpen: s.onOpen,
      setAction: s.setAction,
      setDescription: s.setDescription
    }))
  );

  const handleRouteToAdd = useCallback(() => {
    startTransition(() =>
      router.push(`/playlist/${playlistId}/add?searchString=&type=all`)
    );
  }, [playlistId, router]);

  const deletePlaylistCallback = useCallback(async () => {
    const success = await deletePlaylist(playlistId);
    if (success) {
      router.push("/");
      toast.success("The playlist was successfully deleted");
    } else {
      toast.error("An error occurred while deleting the playlist");
    }
  }, [playlistId, router]);

  const handleDeletePlaylist = useCallback(() => {
    startTransition(() => {
      setDescription("This action will delete this playlist from your library");
      setAction(deletePlaylistCallback);
      onOpen();
    });
  }, [deletePlaylistCallback, onOpen, setAction, setDescription]);

  return (
    <OptionsMenu
      buttonAriaLabel="Actions with playlist"
      align={"left"}
      iconSize={30}
      disabled={disabled || isPending}
      options={[
        {
          id: "add-song-to-playlist",
          label: "Add songs to playlist",
          icon: <AiOutlinePlusCircle />,
          disabled: disabled || isPending,
          onSelect: handleRouteToAdd,
        },
        {
          id: "delete-playlist",
          label: "Delete this playlist",
          icon: <FaTrash />,
          isDestructive: true,
          disabled: disabled || isPending,
          onSelect: handleDeletePlaylist,
        },
      ]}
    />
  );
};

export default PlaylistActionsMenu;
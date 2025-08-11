"use client";

import {useRouter} from "next/navigation";
import {useLayoutEffect, useTransition} from "react";
import { useShallow } from "zustand/shallow";
import toast from "react-hot-toast";

import {useUser} from "@/hooks/useUser";
import usePlaylistModal from "@/hooks/usePlaylistModal";

import Modal from "./Modal";
import Input from "./Input";
import Button from "./Button";

import updatePlaylist from "@/services/playlists/updatePlaylist";
import Form from "next/form";

const PlaylistModal = () => {
  const [playlist, isOpen, onClose, setPlaylist] = usePlaylistModal(useShallow(s => [
    s.playlist,
    s.isOpen,
    s.onClose,
    s.setPlaylist
  ]));
  const { isAuth } = useUser();
  const router = useRouter();
  const [isPending, startTransition] = useTransition();

  useLayoutEffect(() => {
    if (!isAuth || !playlist) {
      router.refresh();
      onClose();
    }
  }, [isAuth, router, onClose, playlist]);
  
  if (!playlist) {
    onClose();
    return;
  }

  const onChange = (open: boolean) => {
    if (!open) {
      onClose();
    }
  }

  const onSubmit = async (formData: FormData) => {
    startTransition(async () => {
      if (!isAuth) {
        toast.error("The user is not authorized!");
        onClose();
        return;
      }
      
      const updateResponse = await updatePlaylist(playlist.id, formData);

      if (!updateResponse.ok) {
        toast.error("An error occurred while updating the playlist");
        return;
      }

      router.refresh();
      toast.success('Playlist information saved!');
      onClose();
      setPlaylist(undefined);
    });
  }

  return (
    <Modal
      title="Edit playlist information"
      description=""
      isOpen={isOpen}
      onChange={onChange}
    >
      <Form
        action={onSubmit}
        className="flex flex-col gap-y-4"
      >
        <Input
          id="title"
          name="Title"
          type="text"
          disabled={isPending}
          placeholder="Playlist Title"
          defaultValue={playlist.title}
          required
        />
        <Input
          id="description"
          name="Description"
          type="text"
          disabled={isPending}
          placeholder="Playlist Description"
          defaultValue={playlist.description}
        />
        <div>
          <label className="pb-1">Select an image</label>
          <Input
            id="image"
            name="Image"
            type="file"
            disabled={isPending}
            accept="image/*"
          />
        </div>
        <Button disabled={isPending} type="submit">
          Edit
        </Button>
      </Form>
    </Modal>
  );
}

export default PlaylistModal;

"use client";

import {useRouter} from "next/navigation";
import {FieldValues, SubmitHandler, useForm} from "react-hook-form";
import {useLayoutEffect, useTransition} from "react";
import { useShallow } from "zustand/shallow";
import toast from "react-hot-toast";

import {useUser} from "@/hooks/useUser";
import usePlaylistModal from "@/hooks/usePlaylistModal";

import Modal from "./Modal";
import Input from "./Input";
import Button from "./Button";

import uploadFile from "@/services/files/uploadFile";
import deleteFile from "@/services/files/deleteFile";
import updatePlaylist from "@/services/playlists/updatePlaylist";

const PlaylistModal = () => {
  const [playlist, isOpen, onClose] = usePlaylistModal(useShallow(s => [
    s.playlist,
    s.isOpen,
    s.onClose
  ]));
  const { isAuth } = useUser();
  const router = useRouter();
  const [isPending, startTransition] = useTransition();

  const {register, handleSubmit, reset, setValue} = useForm<FieldValues>({
    defaultValues: {
      title: "",
      description: "",
      image: null
    }
  });

  useLayoutEffect(() => {
    setValue("title", playlist?.title);
    setValue("description", playlist?.description);
    setValue("image", playlist?.imagePath);
  }, [playlist, setValue]);

  if (!playlist) {
    onClose();
    return;
  }

  const onChange = (open: boolean) => {
    if (!open) {
      reset();
      onClose();
    }
  }

  const onSubmit: SubmitHandler<FieldValues> = async (values) => {
    startTransition(async () => {
      try {
        const imageFile = values.image?.[0];
        let imageFilePath = "";

        if (!isAuth) {
          toast.error("The user is not authorized!");
          onClose();
          return;
        }

        if (!values.title) {
          toast.error('Missing required fields');
          return;
        }

        // TODO:cache control
        // Upload image file
        if (imageFile) {
          const imageForm = new FormData();
          imageForm.append("image", imageFile);

          const imageUploadResponse = await uploadFile(imageForm, "image");

          if (imageUploadResponse.ok) {
            imageFilePath = await imageUploadResponse.json();

            const imageDeleteResponse = await deleteFile(playlist.imagePath ?? "");

            if (!imageDeleteResponse.ok) {
              toast.error("An error occurred while deleting old image file.");
              return
            }
          } else {
            toast.error("An error occurred while uploading image file.")
            return
          }
        }

        if (imageFilePath === "") {
          imageFilePath = playlist.imagePath ?? ""; // image is variable from store
        }

        const updateResponse = await updatePlaylist(
          playlist.id,
          values.title,
          values.description ?? null,
          imageFilePath
        );

        if (!updateResponse.ok) {
          toast.error("An error occurred while updating the playlist info");
          return;
        }

        router.refresh();
        toast.success('Playlist information saved!');
        reset();
        onClose();
      } catch {
        toast.error('Something went wrong');
      }
    });
  }

  return (
    <Modal
      title="Edit playlist information"
      description=""
      isOpen={isOpen}
      onChange={onChange}
    >
      <form
        onSubmit={handleSubmit(onSubmit)}
        className="flex flex-col gap-y-4"
      >
        <Input
          id="title"
          disabled={isPending}
          {...register('title', { required: true })}
          placeholder="Playlist Title"
        />
        <Input
          id="description"
          disabled={isPending}
          {...register('description', { required: false })}
          placeholder="Playlist Description"
        />
        <div>
          <div className="pb-1">
            Select an image
          </div>
          <Input
            id="image"
            type="file"
            disabled={isPending}
            accept="image/*"
            {...register('image', { required: false })}
          />
        </div>
        <Button disabled={isPending} type="submit">
          Edit
        </Button>
      </form>
    </Modal>
  );
}

export default PlaylistModal;

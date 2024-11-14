"use client";

import {useRouter} from "next/navigation";
import {FieldValues, SubmitHandler, useForm} from "react-hook-form";
import {useState} from "react";
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
  const [isLoading, setIsLoading] = useState<boolean>();
  const playlistModal = usePlaylistModal();
  const { isAuth } = useUser();
  const router = useRouter();

  const {
    register,
    handleSubmit,
    reset
  } = useForm<FieldValues>({
    defaultValues: {
      title: playlistModal.title,
      description: playlistModal.description,
      image: null
    }
  });

  const onChange = (open: boolean) => {
    if (!open) {
      reset();
      playlistModal.onClose();
    }
  }

  const onSubmit: SubmitHandler<FieldValues> = async (values) => {
    try {
      setIsLoading(true);
      const imageFile = values.image?.[0];
      let imageFilePath = "";

      if (!isAuth) {
        toast.error("The user is not authorized!");
        playlistModal.onClose();
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

        if (!imageUploadResponse.ok) {
          imageFilePath = await imageUploadResponse.json();

          const imageDeleteResponse = await deleteFile(playlistModal.imagePath);

          if (!imageDeleteResponse.ok) {
            setIsLoading(false);
            return toast.error("An error occurred while deleting old image file.");
          }
        } else {
          setIsLoading(false);
          return toast.error("An error occurred while uploading image file.")
        }
      }

      if (imageFilePath === "") {
        imageFilePath = playlistModal.imagePath;
      }

      const updateResponse = await updatePlaylist(
        playlistModal.id,
        values.title,
        values.description ?? null,
        imageFilePath
      );

      if (!updateResponse.ok) {
        setIsLoading(false);
        return toast.error("An error occurred while updating the playlist info");
      }

      router.refresh();
      setIsLoading(false);
      toast.success('Playlist information saved!');
      reset();
      playlistModal.onClose();
    } catch {
      toast.error('Something went wrong');
    } finally {
      setIsLoading(false);
    }
  }

  return (
    <Modal
      title="Edit playlist information"
      description=""
      isOpen={playlistModal.isOpen}
      onChange={onChange}
    >
      <form
        onSubmit={handleSubmit(onSubmit)}
        className="flex flex-col gap-y-4"
      >
        <Input
          id="title"
          disabled={isLoading}
          {...register('title', { required: true })}
          placeholder="Playlist Title"
        />
        <Input
          id="description"
          disabled={isLoading}
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
            disabled={isLoading}
            accept="image/*"
            {...register('image', { required: false })}
          />
        </div>
        <Button disabled={isLoading} type="submit">
          Edit
        </Button>
      </form>
    </Modal>
  );
}

export default PlaylistModal;

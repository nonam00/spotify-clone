"use client";

import { useRouter } from "next/navigation";
import { FieldValues, SubmitHandler, useForm } from "react-hook-form";
import { useState } from "react";
import toast from "react-hot-toast";

import $api from "@/api/http";

import { useUser } from "@/hooks/useUser";
import usePlaylistModal from "@/hooks/usePlaylistModal";

import Modal from "./Modal";  
import Input from "./Input";
import Button from "./Button";

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
    if(!open) {
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

      if(!values.title) {
        toast.error('Missing required fields');
        return;
      }
      
      // TODO:cache control
      // Upload image file
      if (imageFile) {
        await $api.post("/files/image", { image: imageFile }, {
          headers: {
            "Content-Type": "multipart/form-data"
          }
        })
          .then(response => imageFilePath = response.data.path)
          .catch(() => {
            setIsLoading(false);
            return toast.error("An error occurred while uploading image file.");
          });
      }
      if (imageFilePath === "") {
        imageFilePath = playlistModal.imagePath;
      }

      await $api.put(`/playlists/${playlistModal.id}`, {
        title: values.title,
        description: values.description ?? null,
        imagePath: imageFilePath,
      })
        .catch(() => {
          setIsLoading(false);
          return toast.error("An error occurred while updating the playlist info");
        });

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
          {...register('description', { required: false})}
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

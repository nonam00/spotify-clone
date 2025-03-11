"use client";

import {useRouter} from "next/navigation";
import {FieldValues, SubmitHandler, useForm} from "react-hook-form";
import {useTransition} from "react";
import { useShallow } from "zustand/shallow";
import toast from "react-hot-toast";

import {useUser} from "@/hooks/useUser";
import useUploadModal from "@/hooks/useUploadModal";

import Modal from "./Modal";
import Input from "./Input";
import Button from "./Button";
import uploadSong from "@/services/songs/uploadSong";
import uploadFile from "@/services/files/uploadFile";

const UploadModal = () => {
  const [isPending, startTransition] = useTransition();
  const [onClose, isOpen] = useUploadModal(useShallow(s => [s.onClose, s.isOpen]));
  const { isAuth } = useUser();
  const router = useRouter();

  const {
    register,
    handleSubmit,
    reset
  } = useForm<FieldValues>({
    defaultValues: {
      author: '',
      title: '',
      song: null,
      image: null
    }
  });

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
        const songFile = values.song?.[0];
        let songFilePath = "";
        let imageFilePath = "";

        if (!isAuth) {
          toast.error("The user is not authorized!");
          onClose();
          return;
        }

        if (!imageFile || !songFile) {
          toast.error('Missing required fields');
          return;
        }

        // Upload song file
        const songForm = new FormData();
        songForm.append("song", songFile);

        const songUploadResponse = await uploadFile(songForm, "song");

        if (songUploadResponse.ok) {
          const songUploadData = await songUploadResponse.json();
          songFilePath = songUploadData.path;
        } else {
          toast.error("An error occurred while uploading song file.");
          return;
        }

        // Upload image file
        const imageForm = new FormData();
        imageForm.append("image", imageFile);

        const imageUploadResponse = await uploadFile(imageForm, "image");

        if (imageUploadResponse.ok) {
          const imageUploadData = await imageUploadResponse.json();
          imageFilePath = imageUploadData.path;
        } else {
          toast.error("An error occurred while uploading image file.");
          return
        }

        if (imageFilePath === "" || songFilePath === "") {
          toast.error("An error occurred while uploading files.")
          return
        }

        const response = await uploadSong(
          values.title,
          values.author,
          songFilePath,
          imageFilePath
        );

        if (!response.ok) {
          toast.error("An error occurred while uploading song information");
          return
        }

        router.refresh();
        toast.success('Song created!');
        reset();
        onClose();
      } catch {
        toast.error('Something went wrong');
      }
    });
  }

  return (
    <Modal
      title="Add a song"
      description="Upload a file"
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
          placeholder="Song Title"
        />
        <Input
          id="author"
          disabled={isPending}
          {...register('author', { required: true })}
          placeholder="Song Author"
        />
        <div>
          <div className="pb-1">
            Select a song file
          </div>
          <Input
            id="song"
            type="file"
            disabled={isPending}
            accept=".mp3,.wav,.flac,.m4a,.aac,.ogg"
            {...register('song', { required: true })}
          />
        </div>
        <div>
          <div className="pb-1">
            Select an image
          </div>
          <Input
            id="image"
            type="file"
            disabled={isPending}
            accept="image/*"
            {...register('image', { required: true })}
          />
        </div>
        <Button
          disabled={isPending}
          type="submit"
          className="my-4"
        >
          Create
        </Button>
      </form>
    </Modal>
  );
}

export default UploadModal;

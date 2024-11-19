"use client";

import {useRouter} from "next/navigation";
import {FieldValues, SubmitHandler, useForm} from "react-hook-form";
import {useState} from "react";
import toast from "react-hot-toast";

import {useUser} from "@/hooks/useUser";
import useUploadModal from "@/hooks/useUploadModal";

import Modal from "./Modal";
import Input from "./Input";
import Button from "./Button";
import uploadSong from "@/services/songs/uploadSong";
import uploadFile from "@/services/files/uploadFile";

const UploadModal = () => {
  const [isLoading, setIsLoading] = useState<boolean>();
  const [onClose, isOpen] = useUploadModal(s => [s.onClose, s.isOpen]);
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
    try {
      setIsLoading(true);
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
      if (!songUploadResponse.ok) {
        songFilePath = await songUploadResponse.json();
      } else {
        setIsLoading(false);
        return toast.error("An error occurred while uploading song file.");
      }

      // Upload image file
      const imageForm = new FormData();
      imageForm.append("image", imageFile);

      const imageUploadResponse = await uploadFile(imageForm, "image");

      if (!imageUploadResponse.ok) {
        imageFilePath = await imageUploadResponse.json();
      } else {
        setIsLoading(false);
        return toast.error("An error occurred while uploading image file.");
      }

      if (imageFilePath === "" || songFilePath === "") {
        return toast.error("An error occurred while uploading files.")
      }

      const response = await uploadSong(
        values.title,
        values.author,
        imageFilePath,
        songFilePath
      );

      if (!response.ok) {
        setIsLoading(false);
        return toast.error("An error occurred while uploading song information");
      }

      router.refresh();
      setIsLoading(false);
      toast.success('Song created!');
      reset();
      onClose();
    } catch {
      toast.error('Something went wrong');
    } finally {
      setIsLoading(false);
    }
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
          disabled={isLoading}
          {...register('title', { required: true })}
          placeholder="Song Title"
        />
        <Input
          id="author"
          disabled={isLoading}
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
            disabled={isLoading}
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
            disabled={isLoading}
            accept="image/*"
            {...register('image', { required: true })}
          />
        </div>
        <Button
          disabled={isLoading}
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

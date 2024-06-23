"use client";

import { useRouter } from "next/navigation";
import { FieldValues, SubmitHandler, useForm } from "react-hook-form";
import { useState } from "react";
import toast from "react-hot-toast";

import $api from "@/api/http";

import { useUser } from "@/hooks/useUser";
import useUploadModal from "@/hooks/useUploadModal";

import Modal from "./Modal";  
import Input from "./Input";
import Button from "./Button";

const UploadModal = () => {
  const [isLoading, setIsLoading] = useState<boolean>();
  const uploadModal = useUploadModal();
  const user = useUser();
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
    if(!open) {
      reset();
      uploadModal.onClose();
    }
  }

  const onSubmit: SubmitHandler<FieldValues> = async (values) => {
    try {
      setIsLoading(true);
      const imageFile = values.image?.[0];
      const songFile = values.song?.[0];
      let songFilePath = "";
      let imageFilePath = "";

      if(!imageFile || !songFile || !user.isAuth) {
        toast.error('Missing required fields');
        return;
      }
      
      // TODO:cache control
      // Upload song file
      try {
        await $api.post("/files/song", { song: songFile }, {
          headers: {
            "Content-Type": "multipart/form-data"
          }
        }).then((response) => {
          songFilePath = response.data.path;
        });
      } catch (e){
        setIsLoading(false);
        return toast.error('Failed song upload.')
      }

      // Upload image file
      try {
        await $api.post("/files/image", { image: imageFile }, {
          headers: {
            "Content-Type": "multipart/form-data"
          }
        }).then((response) => {
          imageFilePath = response.data.path;
        })
      } catch {
        setIsLoading(false);
        return toast.error('Failed image upload.')
      }
      if (imageFilePath === "" || songFilePath === "") {
        return toast.error('Failed image or song upload');
      }
      // TODO: fix sending request before files uploaded
      try {
        await $api.post<string>("/songs/post", {
          title: values.title,
          author: values.author,
          imagePath: imageFilePath,
          songPath: songFilePath
        });
      } catch {
        setIsLoading(false);
        return toast.error("Failed song info upload");
      }

      router.refresh();
      setIsLoading(false);
      toast.success('Song created!');
      reset();
      uploadModal.onClose();  
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
      isOpen={uploadModal.isOpen}
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

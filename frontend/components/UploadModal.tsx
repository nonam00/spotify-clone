"use client";

import {useRouter} from "next/navigation";
import Form from "next/form";
import {useLayoutEffect, useTransition} from "react";
import { useShallow } from "zustand/shallow";
import toast from "react-hot-toast";

import {useUser} from "@/hooks/useUser";
import useUploadModal from "@/hooks/useUploadModal";

import Modal from "./Modal";
import Input from "./Input";
import Button from "./Button";
import uploadSong from "@/services/songs/uploadSong";

const UploadModal = () => {
  const [isPending, startTransition] = useTransition();
  const [onClose, isOpen] = useUploadModal(useShallow(s => [s.onClose, s.isOpen]));
  const { isAuth } = useUser();
  const router = useRouter();

  useLayoutEffect(() => {
    if (!isAuth) {
      router.refresh();
      onClose();
    }
  }, [isAuth, router, onClose]);

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

      const response = await uploadSong(formData);

      if (!response.ok) {
        toast.error("An error occurred while uploading song");
        return
      }

      router.refresh();
      toast.success('Song created!');
      onClose();
    });
  }

  return (
    <Modal
      title="Add a song"
      description="Upload a file"
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
          placeholder="Song Title"
          required
        />
        <Input
          id="author"
          name="Author"
          type="text"
          disabled={isPending}
          placeholder="Song Author"
          required
        />
        <div>
          <label className="pb-1">Select a song file</label>
          <Input
            id="audio"
            name="Audio"
            type="file"
            disabled={isPending}
            accept=".mp3,.wav,.flac,.m4a,.aac,.ogg"
            required
          />
        </div>
        <div>
          <label className="pb-1">Select an image</label>
          <Input
            id="image"
            name="Image"
            type="file"
            disabled={isPending}
            accept="image/*"
            required
          />
        </div>
        <Button
          disabled={isPending}
          type="submit"
          className="my-4"
        >
          Create
        </Button>
      </Form>
    </Modal>
  );
}

export default UploadModal;

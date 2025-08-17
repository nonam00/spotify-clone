"use client";

import {useRouter} from "next/navigation";
import Form from "next/form";
import {useLayoutEffect, useTransition} from "react";
import { useShallow } from "zustand/shallow";
import toast from "react-hot-toast";

import {useUser} from "@/hooks/useUser";
import useUploadModal from "@/hooks/useUploadModal";

import uploadSong from "@/services/songs/uploadSong";
import Button from "@/components/ui/Button";
import Input from "@/components/ui/Input";
import Modal from "@/components/ui/Modal";

const SongUploadModal = () => {
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
      title="Upload new song"
      description="Share your music with other users"
      isOpen={isOpen}
      onChange={onChange}
    >
      <Form
        action={onSubmit}
        className="flex flex-col gap-y-4"
      >
        <div className="flex flex-col gap-y-1">
          <label className="text-base font-bold">
            Title:
          </label>
          <Input
            id="title"
            name="Title"
            type="text"
            disabled={isPending}
            placeholder="Song Title"
            required
          />
        </div>
        <div className="flex flex-col gap-y-1">
          <label className="w-full text-base font-bold">
            Author:
          </label>
          <Input
            id="author"
            name="Author"
            type="text"
            disabled={isPending}
            placeholder="Song Author"
            required
          />
        </div>
        <div className="flex flex-col gap-y-1">
          <label className="text-base font-bold">Select a song file:</label>
          <Input
            id="audio"
            name="Audio"
            type="file"
            disabled={isPending}
            accept=".mp3,.wav,.flac,.m4a,.aac,.ogg"
            required
          />
        </div>
        <div className="flex flex-col gap-y-1">
          <label className="text-base font-bold">Select an image:</label>
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
          {isPending ? "Uploading..." : "Upload"}
        </Button>
      </Form>
    </Modal>
  );
}

export default SongUploadModal;

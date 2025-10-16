"use client";

import {useRouter} from "next/navigation";
import Form from "next/form";
import {useLayoutEffect, useTransition} from "react";
import { useShallow } from "zustand/shallow";
import toast from "react-hot-toast";

import {useUser} from "@/hooks/useUser";
import useUploadModal from "@/hooks/useUploadModal";

import Button from "@/components/ui/Button";
import Input from "@/components/ui/Input";
import Modal from "@/components/ui/Modal";
import {CLIENT_API_URL, CLIENT_FILES_URL} from "@/helpers/api";

async function getPresignedUrl(type: string) {
  return await fetch(`${CLIENT_FILES_URL}/upload-url`,{
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify({ file_type: type }),
  });
}

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

      const presignedUrlImageResponse = await getPresignedUrl("image")

      if (!presignedUrlImageResponse.ok) {
        toast.error("An error occurred while getting presigned url!");
        return
      }

      const presignedUrlAudioResponse = await getPresignedUrl("audio")

      if (!presignedUrlAudioResponse.ok) {
        toast.error("An error occurred while getting presigned url!");
        return
      }

      const presignedUrlImage = await presignedUrlImageResponse.json();
      const presignedUrlAudio = await presignedUrlAudioResponse.json();

      const uploadImageResponse = await fetch(presignedUrlImage.url, {
        method: "PUT",
        headers: {
          "content-type": "image/*",
        },
        body: formData.get("Image"),
      });

      if (!uploadImageResponse.ok) {
        toast.error("An error occurred while uploading image");
        return
      }

      const uploadAudioResponse = await fetch(presignedUrlAudio.url, {
        method: "PUT",
        headers: {
          "content-type": "audio/*",
        },
        body: formData.get("Audio"),
      });

      if (!uploadAudioResponse.ok) {
        toast.error("An error occurred while uploading song");
        return
      }

      const body = {
        title: formData.get("Title"),
        author: formData.get("Author"),
        imageId: presignedUrlImage.file_id,
        audioId: presignedUrlAudio.file_id,
      };

      const response = await fetch(`${CLIENT_API_URL}/songs`, {
        method: "POST",
        headers: {
          "content-type": "application/json",
        },
        body: JSON.stringify(body),
        credentials: "include",
      });

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

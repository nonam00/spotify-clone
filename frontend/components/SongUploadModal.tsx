"use client";

import { useRouter } from "next/navigation";
import Form from "next/form";
import { useLayoutEffect, useTransition } from "react";
import { useShallow } from "zustand/shallow";
import toast from "react-hot-toast";

import { useUser } from "@/hooks/useUser";
import useUploadModal from "@/hooks/useUploadModal";

import Button from "@/components/ui/Button";
import Input from "@/components/ui/Input";
import Modal from "@/components/ui/Modal";
import {FILE_CONFIG, getPresignedUrls, uploadFileToS3, validateAudio, validateImage} from "@/services/files";
import {createSongRecord} from "@/services/songs";

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
  };

  const onSubmit = async (formData: FormData) => {
    startTransition(async () => {
      if (!isAuth) {
        toast.error("The user is not authorized!");
        onClose();
        return;
      }

      const title = formData.get("Title") as string;
      const author = formData.get("Author") as string;
      const imageFile = formData.get("Image") as File;
      const audioFile = formData.get("Audio") as File;

      if (!title?.trim() || !author?.trim()) {
        toast.error("Title and author are required");
        return;
      }

      if (!imageFile || !audioFile) {
        toast.error("Please select both image and audio files");
        return;
      }

      const imageError = validateImage(imageFile);
      if (imageError) {
        toast.error(`Image error: ${imageError}`);
        return;
      }

      const audioError = validateAudio(audioFile);
      if (audioError) {
        toast.error(`Audio error: ${audioError}`);
        return;
      }

      const urls = await getPresignedUrls();
      if (!urls) {
        toast.error("Failed to get upload URLs");
        return;
      }

      const [presignedUrlImage, presignedUrlAudio] = urls;

      const [imageUploadSuccess, audioUploadSuccess] = await Promise.all([
        uploadFileToS3(presignedUrlImage.url, imageFile, "image"),
        uploadFileToS3(presignedUrlAudio.url, audioFile, "audio"),
      ]);

      if (!imageUploadSuccess || !audioUploadSuccess) {
        toast.error("Failed to upload files");
        return;
      }

      const songData = {
        title: title.trim(),
        author: author.trim(),
        imageId: presignedUrlImage.file_id,
        audioId: presignedUrlAudio.file_id,
      };

      const recordSuccess = await createSongRecord(songData);
      if (!recordSuccess) {
        toast.error("Failed to create song record");
        return;
      }

      router.refresh();
      toast.success('Song created successfully!');
      onClose();
    });
  };

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
                minLength={1}
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
                minLength={1}
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
            <p className="text-xs text-gray-500 mt-1">
              Max size: {Math.round(FILE_CONFIG.audio.maxSize / 1024 / 1024)}MB •
              Supported formats: MP3, WAV, FLAC, M4A, AAC, OGG
            </p>
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
            <p className="text-xs text-gray-500 mt-1">
              Max size: {Math.round(FILE_CONFIG.image.maxSize / 1024 / 1024)}MB •
              Supported formats: JPEG, PNG, WebP, GIF
            </p>
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
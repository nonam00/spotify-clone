"use client";

import {  type SubmitEvent, useCallback, useState, useTransition } from "react";
import { useShallow } from "zustand/shallow";
import { z } from "zod";
import toast from "react-hot-toast";

import {
  audioFileSchema,
  imageFileSchema,
  FILE_CONFIG,
  getPresignedUrls,
  uploadFileToS3
} from "@/shared/lib/files";
import { Button, Input, Modal } from "@/shared/ui";
import { createSongRecord } from "@/entities/song";
import { useAuthStore } from "@/features/auth";
import { useUploadModalStore } from "../model";

const initialFormData = {
  title: "",
  author: "",
  audioFile: new File([], ""),
  imageFile: new File([], ""),
}

const songFormSchema = z.object({
  title: z.string()
    .trim()
    .min(1, "Title is required")
    .max(255, "Title must be less than 255 characters"),
  author: z.string()
    .trim()
    .min(1, "Author is required")
    .max(255, "Author must be less than 255 characters"),
  audioFile: audioFileSchema,
  imageFile: imageFileSchema,
});

type SongFormData = z.infer<typeof songFormSchema>;

const SongUploadModal = () => {
  const [isPending, startTransition] = useTransition();

  const { onClose, isOpen } = useUploadModalStore(
    useShallow((s) => ({
      onClose: s.onClose,
      isOpen: s.isOpen,
    }))
  );

  const isAuthenticated = useAuthStore(useShallow((s) => s.isAuthenticated));

  const [formData, setFormData] = useState<SongFormData>({...initialFormData});
  const [showErrors, setShowErrors] = useState<boolean>(false);

  const onChange = useCallback((open: boolean) => {
    if (!open) {
      onClose();
    }
  }, [onClose]);

  const validate = () => {
    const result = songFormSchema.safeParse(formData);
    if (result.success) {
      return undefined;
    }
    return z.flattenError(result.error);
  }

  const onSubmit = async (e: SubmitEvent) => {
    e.preventDefault();

    if (!isAuthenticated) {
      toast.error("The user is not authorized!");
      onClose();
      return;
    }

    startTransition(async () => {
      const errors = validate();
      if (errors) {
        setShowErrors(true);
        return;
      }

      const urls = await getPresignedUrls();
      if (!urls) {
        toast.error("Failed to get upload URLs");
        return;
      }

      const [presignedUrlImage, presignedUrlAudio] = urls;

      const [imageUploadSuccess, audioUploadSuccess] = await Promise.all([
        uploadFileToS3(presignedUrlImage.url, formData.imageFile, "image"),
        uploadFileToS3(presignedUrlAudio.url, formData.audioFile, "audio"),
      ]);

      if (!imageUploadSuccess || !audioUploadSuccess) {
        toast.error("Failed to upload files");
        return;
      }

      const songData = {
        title: formData.title.trim(),
        author: formData.author.trim(),
        imageId: presignedUrlImage.file_id,
        audioId: presignedUrlAudio.file_id,
      };

      const recordSuccess = await createSongRecord(songData);
      if (!recordSuccess) {
        toast.error("Failed to create song record");
        return;
      }

      toast.success("Song uploaded successfully! Waiting for moderation...");
      setFormData({...initialFormData});
      onClose();
    });
  };

  const errors = showErrors ? validate() : undefined;

  return (
    <Modal
      title="Upload new song"
      description="Share your music with other users"
      isOpen={isOpen}
      onChange={onChange}
    >
      <form onSubmit={onSubmit} className="flex flex-col gap-y-1">
        <label className="flex flex-col gap-y-1 text-base font-bold">
          Title:
          <Input
            value={formData.title}
            onChange={(e) =>
              setFormData(prev => ({...prev, title: e.target.value}))
            }
            type="text"
            disabled={isPending}
            placeholder="Enter song title..."
            required
            minLength={1}
          />
          <p className={`text-red-500 text-sm mt-1 ${errors?.fieldErrors.title ? "visible" : "invisible"}`}>
            {errors?.fieldErrors.title?.join(", ") ?? "empty"}
          </p>
        </label>
        <label className="flex flex-col gap-y-1 text-base font-bold">
          Author:
          <Input
            value={formData.author}
            onChange={(e) =>
              setFormData(prev => ({...prev, author: e.target.value}))
            }
            type="text"
            disabled={isPending}
            placeholder="Enter song author..."
            required
            minLength={1}
          />
          <p className={`text-red-500 text-xs mt-1 ${errors?.fieldErrors.author ? "visible" : "invisible"}`}>
            {errors?.fieldErrors.author?.join(", ") ?? "empty"}
          </p>
        </label>
        <label className="flex flex-col gap-y-1 text-base font-bold">
          Select a song file:
          <Input
            onChange={(e) => {
              if (e.target.files?.[0]) {
                setFormData({...formData, audioFile: e.target.files?.[0]});
              }
            }}
            type="file"
            disabled={isPending}
            accept={FILE_CONFIG.audio.allowedTypes.join(", ")}
            required
          />
          <p className="text-xs text-gray-500 mt-1">
            Max size: {Math.round(FILE_CONFIG.audio.maxSize / 1024 / 1024)}MB
            • Supported formats: {FILE_CONFIG.audio.allowedTypes.join(", ")}
          </p>
          <p className={`text-red-500 text-sm mt-1 ${errors?.fieldErrors.audioFile ? "visible" : "invisible"}`}>
            {errors?.fieldErrors.audioFile?.join(", ") ?? "empty"}
          </p>
        </label>
        <label className="flex flex-col gap-y-1 text-base font-bold">
          Select an image:
          <Input
            onChange={(e) => {
              if (e.target.files?.[0]) {
                setFormData({...formData, imageFile: e.target.files?.[0]});
              }
            }}
            type="file"
            disabled={isPending}
            accept={FILE_CONFIG.image.allowedTypes.join(", ")}
            required
          />
          <p className="text-xs text-gray-500 mt-1">
            Max size: {Math.round(FILE_CONFIG.image.maxSize / 1024 / 1024)}MB
            • Supported formats: {FILE_CONFIG.image.allowedTypes.join(", ")}
          </p>
          <p className={`text-red-500 text-xs mt-1 ${errors?.fieldErrors.imageFile ? "visible" : "invisible"}`}>
            {errors?.fieldErrors.imageFile?.join(", ") ?? "empty"}
          </p>
        </label>
        <Button disabled={isPending} type="submit" className="my-4">
          {isPending ? "Uploading..." : "Upload"}
        </Button>
      </form>
    </Modal>
  );
};

export default SongUploadModal;
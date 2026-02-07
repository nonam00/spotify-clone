"use client";

import { useRouter } from "next/navigation";
import { type SubmitEvent, useCallback, useState, useTransition } from "react";
import { useShallow } from "zustand/shallow";
import { z } from "zod";
import toast from "react-hot-toast";

import {
  FILE_CONFIG,
  imageFileSchema,
  getPresignedUrl,
  uploadFileToS3,
} from "@/shared/lib/files";
import { Button, Input, Modal } from "@/shared/ui";
import { updatePlaylist } from "@/entities/playlist";
import { useAuthStore } from "@/features/auth";
import { useUpdatePlaylistModalStore } from "../model";

const initialFormState = {
  title: "",
  description: "",
  imageFile: null,
}

const playlistFormSchema = z.object({
  title: z.string()
    .trim()
    .min(1, "Title is required")
    .max(255, "Title must be less than 255 characters"),
  description: z.string()
    .trim()
    .max(1000, "Title must be less than 1000 characters")
    .optional()
    .nullable(),
  imageFile: imageFileSchema.optional().nullable(),
});

type PlaylistFormData = z.infer<typeof playlistFormSchema>;

const UpdatePlaylistModal = () => {
  const router = useRouter();
  const [isPending, startTransition] = useTransition();

  const { playlist, isOpen, onClose, setPlaylist } = useUpdatePlaylistModalStore(
    useShallow((s) => ({
      playlist: s.playlist,
      isOpen: s.isOpen,
      onClose: s.onClose,
      setPlaylist: s.setPlaylist,
    }))
  );

  const isAuthenticated = useAuthStore(useShallow((s) => s.isAuthenticated));

  const [userFormData, setUserFormData] = useState<Partial<PlaylistFormData>>({});

  const [showErrors, setShowErrors] = useState<boolean>(false);

  const onChange = useCallback((open: boolean) => {
    if (!open) {
      onClose();
      setUserFormData({});
    }
  }, [onClose]);

  if (!playlist || !isAuthenticated || !isOpen) {
    onClose();
    return;
  }

  const formData: PlaylistFormData = {
    ...initialFormState,
    ...playlist,
    ...userFormData,
  }

  const validate = () => {
    const result = playlistFormSchema.safeParse(formData);
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

      if (
        (!formData.description && !playlist.description || formData.description?.trim() === playlist.description) &&
        formData.title.trim() === playlist.title &&
        !formData.imageFile
      ) {
        toast.error("There are no changes");
        return;
      }

      let file_id: string | null = null;

      if (formData.imageFile) {
        const presignedUrlImage = await getPresignedUrl("image");
        if (!presignedUrlImage) {
          toast.error("Failed to get upload URL");
          return;
        }

        const imageUploadSuccess = await uploadFileToS3(
          presignedUrlImage.url,
          formData.imageFile,
          "image"
        );

        if (!imageUploadSuccess) {
          toast.error("Failed to upload files");
          return;
        }

        file_id = presignedUrlImage.file_id;
      }

      const playlistData = {
        title: formData.title.trim(),
        description: formData.description?.trim() || null,
        imageId: file_id,
      };

      const recordSuccess = await updatePlaylist(playlist.id, playlistData);
      if (!recordSuccess) {
        toast.error("Failed to update playlist");
        return;
      }

      router.refresh();
      toast.success("Playlist information saved!");
      onClose();
      setPlaylist(undefined);
      setUserFormData({});
    });
  };

  const errors = showErrors ? validate() : undefined;

  return (
    <Modal
      title="Edit playlist information"
      description=""
      isOpen={isOpen}
      onChange={onChange}
    >
      <form onSubmit={onSubmit} className="flex flex-col gap-y-1">
        <div>
          <label className="flex flex-col gap-y-1 text-base font-bold">
            Title:
            <Input
              value={formData.title}
              onChange={(e) =>
                setUserFormData(prev => ({...prev, title: e.target.value}))
              }
              type="text"
              disabled={isPending}
              placeholder="Enter playlist title..."
              required
              minLength={1}
              maxLength={255}
            />
            <p className={`text-red-500 text-sm mt-1 ${errors?.fieldErrors.title ? "visible" : "invisible"}`}>
              {errors?.fieldErrors.title?.join(", ") ?? "empty"}
            </p>
          </label>
          <label className="flex flex-col gap-y-1 text-base font-bold">
            Description:
            <Input
              value={formData.description ?? ""}
              onChange={(e) =>
                setUserFormData(prev => ({...prev, description: e.target.value}))
              }
              type="text"
              disabled={isPending}
              placeholder="Enter playlist description..."
              maxLength={1000}
            />
            <p className={`text-red-500 text-xs mt-1 ${errors?.fieldErrors.description ? "visible" : "invisible"}`}>
              {errors?.fieldErrors.description?.join(", ") ?? "empty"}
            </p>
          </label>
          <label className="flex flex-col gap-y-1 text-base font-bold">
            Select playlist cover:
            <Input
              onChange={(e) => {
                if (e.target.files?.[0]) {
                  setUserFormData({...formData, imageFile: e.target.files?.[0]});
                }
              }}
              type="file"
              disabled={isPending}
              accept={FILE_CONFIG.image.allowedTypes.join(", ")}
            />
            <p className="text-xs text-gray-500 mt-1">
              Max size: {Math.round(FILE_CONFIG.image.maxSize / 1024 / 1024)}MB
              • Supported formats: {FILE_CONFIG.image.allowedTypes.join(", ")}
            </p>
            <p className={`text-red-500 text-xs mt-1 ${errors?.fieldErrors.imageFile ? "visible" : "invisible"}`}>
              {errors?.fieldErrors.imageFile?.join(", ") ?? "empty"}
            </p>
          </label>
        </div>
        <Button disabled={isPending} type="submit">
          {isPending ? "Saving..." : "Save changes"}
        </Button>
      </form>
    </Modal>
  );
};

export default UpdatePlaylistModal;
"use client";

import {useRouter} from "next/navigation";
import Form from "next/form";
import {useLayoutEffect, useTransition} from "react";
import { useShallow } from "zustand/shallow";
import toast from "react-hot-toast";

import {useUser} from "@/hooks/useUser";
import usePlaylistModal from "@/hooks/usePlaylistModal";

import Button from "@/components/ui/Button";
import Input from "@/components/ui/Input";
import Modal from "@/components/ui/Modal";
import {updatePlaylist} from "@/services/playlists";
import {FILE_CONFIG, getPresignedUrl, uploadFileToS3, validateImage} from "@/services/files";

const UpdatePlaylistModal = () => {
  const [playlist, isOpen, onClose, setPlaylist] = usePlaylistModal(useShallow(s => [
    s.playlist,
    s.isOpen,
    s.onClose,
    s.setPlaylist
  ]));
  const { isAuth } = useUser();
  const router = useRouter();
  const [isPending, startTransition] = useTransition();

  useLayoutEffect(() => {
    if (!isAuth || !playlist) {
      router.refresh();
      onClose();
    }
  }, [isAuth, router, onClose, playlist]);
  
  if (!playlist) {
    onClose();
    return;
  }

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

      const title = formData.get("Title") as string;
      const description = formData.get("Description") as string;
      const imageFile = formData.get("Image") as File;
      console.log(imageFile);

      if (!title?.trim()) {
        toast.error("Title is required");
        return;
      }

      let file_id = null;

      if (imageFile.size !== 0) {
        const imageError = validateImage(imageFile);
        if (imageError) {
          toast.error(`Image error: ${imageError}`);
          return;
        }

        const presignedUrlImage = await getPresignedUrl("image");
        if (!presignedUrlImage) {
          toast.error("Failed to get upload URL");
          return;
        }

        const imageUploadSuccess = await uploadFileToS3(presignedUrlImage.url, imageFile, "image");

        if (!imageUploadSuccess) {
          toast.error("Failed to upload files");
          return;
        }

        file_id = presignedUrlImage.file_id;
      }

      const playlistData = {
        title: title.trim(),
        description: description?.trim(),
        imageId: file_id,
      };

      const recordSuccess = await updatePlaylist(playlist.id, playlistData);
      if (!recordSuccess) {
        toast.error("Failed to create song record");
        return;
      }

      if (!recordSuccess) {
        toast.error("An error occurred while updating the playlist");
        return;
      }

      router.refresh();
      toast.success('Playlist information saved!');
      onClose();
      setPlaylist(undefined);
    });
  }

  return (
    <Modal
      title="Edit playlist information"
      description=""
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
            placeholder="Playlist Title"
            defaultValue={playlist.title}
            required
          />
        </div>
        <div className="flex flex-col gap-y-1">
          <label className="text-base font-bold">
            Description:
          </label>
          <Input
            id="description"
            name="Description"
            type="text"
            disabled={isPending}
            placeholder="Playlist Description"
            defaultValue={playlist.description}
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
          />
          <p className="text-xs text-gray-500 mt-1">
            Max size: {Math.round(FILE_CONFIG.image.maxSize / 1024 / 1024)}MB •
            Supported formats: JPEG, PNG, WebP, GIF
          </p>
        </div>
        <Button disabled={isPending} type="submit">
          {isPending ? "Saving..." : "Save changes"}
        </Button>
      </Form>
    </Modal>
  );
}

export default UpdatePlaylistModal;

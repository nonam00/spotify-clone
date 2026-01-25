"use client";

import Image from "next/image";
import { ChangeEvent, FormEvent, useRef, useState, useTransition } from "react";
import { useShallow } from "zustand/shallow";
import toast from "react-hot-toast";
import { IoMdCloudUpload } from "react-icons/io";

import {
  FILE_CONFIG,
  getPresignedUrl,
  uploadFileToS3,
  validateImage,
} from "@/shared/lib/files";
import { Input, Button } from "@/shared/ui";
import { type UserDetails, updateUserInfo } from "@/entities/user/";
import { useAuthStore } from "@/features/auth";

const ChangeUserInfoForm = ({
  userDetails,
}: {
  userDetails: UserDetails;
}) => {
  const checkAuth = useAuthStore(useShallow((s) => s.checkAuth));

  const [isPending, startTransition] = useTransition();

  const [fullName, setFullName] = useState(userDetails?.fullName || "");
  const [previewUrl, setPreviewUrl] = useState<string | null>(null);
  const [errors, setErrors] = useState<{ file?: string; name?: string }>({});

  const fileInputRef = useRef<HTMLInputElement>(null);

  const handleFileChange = (e: ChangeEvent<HTMLInputElement>) => {
    const newErrors: typeof errors = errors;
    const file = e.target.files?.[0];

    if (!file) {
      return;
    }

    const validationResult = validateImage(file);
    if (validationResult) {
      newErrors.file = validationResult;
      setErrors(newErrors);
      e.target.value = "";
      return;
    }

    const url = URL.createObjectURL(file);
    setPreviewUrl(url);
  };

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();

    const file = fileInputRef.current?.files?.[0];

    if (fullName === userDetails?.fullName && !file) {
      return;
    }

    startTransition(async () => {
      try {
        const newErrors: typeof errors = errors;

        if (fullName.trim().length < 2) {
          newErrors.name = "Name must be at least 2 characters";
          setErrors(newErrors);
          return;
        }

        let file_id = null;

        if (file) {
          const presignedUrlImage = await getPresignedUrl("image");

          if (!presignedUrlImage) {
            toast.error("Failed to get upload URL");
            return;
          }

          const imageUploadSuccess = await uploadFileToS3(
            presignedUrlImage.url,
            file,
            "image"
          );

          if (!imageUploadSuccess) {
            toast.error("Failed to upload files");
            return;
          }

          file_id = presignedUrlImage.file_id;
        }

        const result = await updateUserInfo({
          avatarId: file_id,
          fullName: fullName === userDetails?.fullName ? null : fullName,
        });
        if (result.success) {
          toast.success("User information updated successfully");
          setPreviewUrl(null);
          if (fileInputRef.current) {
            fileInputRef.current.value = "";
          }
          await checkAuth();
        } else {
          toast.error(result.error || "Failed to update user information");
        }
      } catch (error) {
        toast.error("Something went wrong");
        console.error(error);
      }
    });
  };

  return (
    <div className="bg-neutral-800/50 rounded-lg p-6">
      <h3 className="text-lg font-semibold mb-4">Update user information</h3>
      <form onSubmit={handleSubmit} className="space-y-4">
        <div>
          <div className="flex flex-col items-center justify-center w-full gap-y-4">
            <div className="flex flex-col w-full gap-y-1">
              <p className="w-full text-left">Upload new avatar:</p>
              <label
                className="
                flex flex-col items-center justify-center w-full h-30
                bg-neutral-700 border-2 border-neutral-600 rounded-lg cursor-pointer
                hover:bg-neutral-600 transition
              "
              >
                {previewUrl ? (
                  <Image
                    src={previewUrl}
                    alt="Preview"
                    className="w-24 h-24 rounded-full object-cover my-1"
                    loading="lazy"
                    unoptimized
                    width={96}
                    height={96}
                  />
                ) : (
                  <div className="flex flex-col items-center justify-center py-2">
                    <IoMdCloudUpload className="w-8 h-8 mb-2 text-neutral-400" />
                    <p className="mb-1 text-sm text-neutral-400">
                      <span className="font-semibold">Click to upload</span> or drag and drop
                    </p>
                    <p className="text-xs text-neutral-400">PNG, JPG, GIF up to 5MB</p>
                  </div>
                )}
                <Input
                  ref={fileInputRef}
                  type="file"
                  accept={FILE_CONFIG.image.allowedTypes.join(", ")}
                  className="hidden"
                  onChange={handleFileChange}
                  disabled={isPending}
                />
              </label>
              <p className={`text-red-500 text-sm ${errors.file ? "visible" : "invisible"}`}>
                {errors.file ?? "empty"}
              </p>
            </div>

            <label className="flex flex-col gap-y-1 w-full">
              <p>Full name:</p>
              <Input
                value={fullName}
                onChange={(e) => setFullName(e.target.value)}
                placeholder="Enter your full name"
                disabled={isPending}
              />
              <p className={`text-red-500 text-sm mt-1 ${errors.name ? "visible" : "invisible"}`}>
                {errors.name ?? "empty"}
              </p>
            </label>
          </div>
        </div>
        <Button type="submit" disabled={isPending}>
          {isPending ? "Updating..." : "Update"}
        </Button>
      </form>
    </div>
  );
};

export default ChangeUserInfoForm;
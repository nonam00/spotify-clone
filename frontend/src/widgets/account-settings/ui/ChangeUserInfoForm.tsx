"use client";

import Image from "next/image";
import {
  type DragEvent,
  type ChangeEvent,
  type SubmitEvent,
  useCallback,
  useRef,
  useState,
  useTransition
} from "react";
import { useShallow } from "zustand/shallow";
import { z } from "zod";
import toast from "react-hot-toast";
import { IoMdCloudUpload } from "react-icons/io";

import {
  FILE_CONFIG,
  imageFileSchema,
  getPresignedUrl,
  uploadFileToS3,
} from "@/shared/lib/files";
import { Input, Button } from "@/shared/ui";
import { type UserDetails, updateUserInfo } from "@/entities/user/";
import { useAuthStore } from "@/features/auth";

const initialFormState = {
  fullName: "",
  file: null,
}

const userInfoFormSchema = z.object({
  fullName: z.string()
    .trim()
    .min(2, "Name must be at least 2 characters")
    .max(255, "Name must be less than 255 characters"),
  file: imageFileSchema.optional().nullable(),
});

type UserInfoFormData = z.infer<typeof userInfoFormSchema>;

const ChangeUserInfoForm = ({
  userDetails,
}: {
  userDetails: UserDetails;
}) => {
  const checkAuth = useAuthStore(useShallow((s) => s.checkAuth));
  const [isPending, startTransition] = useTransition();

  const [formData, setFormData] = useState<UserInfoFormData>({
    ...initialFormState,
    fullName: userDetails.fullName ?? "",
  });

  const [previewUrl, setPreviewUrl] = useState<string | null>(null);
  const [showErrors, setShowErrors] = useState<boolean>(false);

  const fileInputRef = useRef<HTMLInputElement>(null);

  const validate = useCallback((formData: UserInfoFormData) => {
    const result = userInfoFormSchema.safeParse(formData);
    if (result.success) {
      return undefined;
    }
    return z.flattenError(result.error);
  }, []);

  const handleFileDrop = (e: DragEvent) => {
    e.preventDefault();
    handleFileChange(e.dataTransfer.files[0]);
  };

  const handleFileChange = (file: File | undefined) => {
    if (!file) {
      return;
    }
    const newFormData = { ...formData, file: file };
    setFormData({ ...formData, file: file });
    const errors = validate(newFormData);
    if (errors) {
      setShowErrors(true);
      setPreviewUrl(null);
      if (fileInputRef.current) {
        fileInputRef.current.value = "";
      }
      return;
    }
    setPreviewUrl(URL.createObjectURL(file));
  };

  const handleFullNameChange = (e: ChangeEvent<HTMLInputElement>) => {
    const newFormData = { ...formData, fullName: e.target.value };
    setFormData(newFormData);
    const errors = validate(formData);
    if (errors) {
      setShowErrors(true);
    }
  };

  const handleSubmit = async (e: SubmitEvent) => {
    e.preventDefault();

    const errors = validate(formData);
    if (errors) {
      setShowErrors(true);
      return;
    }

    // Check if there are any changes
    const trimmedFullName = formData.fullName.trim();
    const isNameChanged = trimmedFullName !== userDetails?.fullName;
    const isFileChanged = formData.file !== null;

    if (!isNameChanged && !isFileChanged) {
      toast.error("No changes to update");
      return;
    }

    startTransition(async () => {
      try {
        let file_id = null;

        if (formData.file) {
          const presignedUrlImage = await getPresignedUrl("image");

          if (!presignedUrlImage) {
            toast.error("Failed to get upload URL");
            return;
          }

          const imageUploadSuccess = await uploadFileToS3(
            presignedUrlImage.url,
            formData.file,
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
          fullName: isNameChanged ? trimmedFullName : null,
        });

        if (result.success) {
          toast.success("User information updated successfully");

          setFormData({
            fullName: trimmedFullName,
            file: null,
          });

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

  const errors = showErrors ? validate(formData) : undefined;

  const isSubmitDisabled =
    isPending ||
    errors !== undefined ||
    (formData.fullName.trim() === userDetails?.fullName && !formData.file);

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
                onDragOver={e => e.preventDefault()}
                onDragEnter={e => e.preventDefault()}
                onDrop={handleFileDrop}
              >
                {previewUrl ? (
                  <div className="relative w-24 h-24 my-1">
                    <Image
                      src={previewUrl}
                      alt="Preview"
                      className="object-cover rounded-full"
                      loading="lazy"
                      unoptimized
                      fill
                    />
                  </div>
                ) : (
                  <div className="flex flex-col items-center justify-center py-2">
                    <IoMdCloudUpload className="w-8 h-8 mb-2 text-neutral-400" />
                    <p className="mb-1 text-sm text-neutral-400">
                      <span className="font-semibold">Click to upload</span> or drag and drop
                    </p>
                    <p className="text-xs text-neutral-400">JPEG, PNG, WebP, HEIF, RAW up to 32MB</p>
                  </div>
                )}
                <Input
                  ref={fileInputRef}
                  type="file"
                  accept={FILE_CONFIG.image.allowedTypes.join(", ")}
                  className="hidden"
                  onChange={(e) => handleFileChange(e.target.files?.[0])}
                  disabled={isPending}
                />
              </label>
              <p className={`text-red-500 text-sm ${errors?.fieldErrors?.file ? "visible" : "invisible"}`}>
                {errors?.fieldErrors?.file?.join(", ") ?? "empty"}
              </p>
            </div>

            <label className="flex flex-col gap-y-1 w-full">
              <p>Full name:</p>
              <Input
                value={formData.fullName}
                onChange={handleFullNameChange}
                placeholder="Enter your full name..."
                disabled={isPending}
              />
              <p className={`text-red-500 text-sm mt-1 ${errors?.fieldErrors?.fullName ? "visible" : "invisible"}`}>
                {errors?.fieldErrors?.fullName?.join(", ") ?? "empty"}
              </p>
            </label>
          </div>
        </div>
        <Button type="submit" disabled={isSubmitDisabled} aria-disabled={isSubmitDisabled}>
          {isPending ? "Updating..." : "Update"}
        </Button>
      </form>
    </div>
  );
};

export default ChangeUserInfoForm;
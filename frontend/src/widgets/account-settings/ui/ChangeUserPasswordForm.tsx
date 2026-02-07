"use client";

import {useState, useTransition, type SubmitEvent, useCallback} from "react";
import { z } from "zod";
import toast from "react-hot-toast";

import { Input, Button } from "@/shared/ui";
import { changePassword } from "@/entities/user";

const initialFormState = {
  currentPassword: "",
  newPassword: "",
  confirmPassword: "",
};

const passwordChangeSchema = z.object({
  currentPassword: z.string()
    .trim()
    .min(8, "Password must be at least 8 characters")
    .max(100, "Password must be less than 100 characters"),
  newPassword: z.string()
    .trim()
    .min(8, "Password must be at least 8 characters")
    .max(100, "Password must be less than 100 characters"),
  confirmPassword: z.string()
    .trim()
    .min(1, "Please confirm your new password"),
}).refine((data) => data.newPassword !== data.currentPassword, {
  message: "New password must be different from current password",
  path: ["newPassword"],
}).refine((data) => data.newPassword === data.confirmPassword, {
  message: "Passwords do not match",
  path: ["confirmPassword"],
});

type PasswordChangeData = z.infer<typeof passwordChangeSchema>;

const ChangeUserPasswordForm = () => {
  const [isPending, startTransition] = useTransition();

  const [formData, setFormData] = useState<PasswordChangeData>({...initialFormState});
  const [showErrors, setShowErrors] = useState<boolean>(false);

  const validate = useCallback((formData: PasswordChangeData) => {
    const result = passwordChangeSchema.safeParse(formData);
    if (result.success) {
      return undefined;
    }
    return z.flattenError(result.error);
  }, []);


  const handleSubmit = async (e: SubmitEvent) => {
    e.preventDefault();

    startTransition(async () => {
      const errors = validate(formData);

      if (errors) {
        setShowErrors(true);
        return;
      }

      try {
        const result = await changePassword(
          formData.currentPassword.trim(),
          formData.newPassword.trim()
        );

        if (result.success) {
          toast.success("Password changed successfully");
          setFormData({...initialFormState});
          setShowErrors(false);
        } else {
          toast.error(result.error || "Failed to change password");
        }
      } catch (error) {
        toast.error("Something went wrong");
        console.error(error);
      }
    });
  };

  const errors = showErrors ? validate(formData) : undefined;

  const isDisabled =
    errors !== undefined ||
    isPending;

  return (
    <div className="bg-neutral-800/50 rounded-lg p-6">
      <h3 className="text-lg font-semibold mb-4">Change Password</h3>
      <form onSubmit={handleSubmit} className="flex flex-col gap-y-2">
        <label className="flex flex-col gap-y-1">
          Current password:
          <Input
            value={formData.currentPassword}
            onChange={(e) =>
              setFormData(prev => ({...prev, currentPassword: e.target.value}))
            }
            type="password"
            placeholder="Enter current password..."
            disabled={isPending}
            required
          />
          <p className={`text-red-500 text-sm mt-1 ${errors?.fieldErrors.currentPassword ? "visible" : "invisible"}`}>
            {errors?.fieldErrors.currentPassword?.join(", ") ?? "empty"}
          </p>
        </label>

        <label className="flex flex-col gap-y-1">
          New password:
          <Input
            value={formData.newPassword}
            onChange={(e) =>
              setFormData(prev => ({...prev, newPassword: e.target.value}))
            }
            type="password"
            placeholder="Enter new password..."
            disabled={isPending}
            required
          />
          <p className={`text-red-500 text-sm mt-1 ${errors?.fieldErrors.newPassword  ? "visible" : "invisible"}`}>
            {errors?.fieldErrors.newPassword?.join(", ") ?? "empty"}
          </p>
        </label>

        <label className="flex flex-col gap-y-1">
          Repeat new password:
          <Input
            value={formData.confirmPassword}
            onChange={(e) =>
              setFormData(prev => ({...prev, confirmPassword: e.target.value}))
            }
            type="password"
            placeholder="Confirm new password..."
            disabled={isPending}
            required
          />
          <p className={`text-red-500 text-sm mt-1 ${errors?.fieldErrors.confirmPassword  ? "visible" : "invisible"}`}>
            {errors?.fieldErrors.confirmPassword?.join(", ") ?? "empty"}
          </p>
        </label>

        <Button type="submit" disabled={isDisabled}>
          {isPending ? "Changing..." : "Change Password"}
        </Button>
      </form>
    </div>
  );
};

export default ChangeUserPasswordForm;
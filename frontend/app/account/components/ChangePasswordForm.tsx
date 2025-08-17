"use client";

import {useState, useTransition} from "react";
import toast from "react-hot-toast";

import changePassword from "@/actions/user/changePassword";
import Input from "@/components/ui/Input";
import Button from "@/components/ui/Button";

const ChangePasswordForm = () => {
  const [isPending, startTransition] = useTransition();
  const [currentPassword, setCurrentPassword] = useState("");
  const [newPassword, setNewPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [errors, setErrors] = useState<{
    currentPassword?: string;
    newPassword?: string;
    confirmPassword?: string;
  }>({});

  const validateForm = () => {
    const newErrors: typeof errors = {};

    if (currentPassword.length < 8) {
      newErrors.currentPassword = "Password must be at least 8 characters";
    }
    if (newPassword.length < 8) {
      newErrors.newPassword = "Password must be at least 8 characters";
    }
    if (confirmPassword !== newPassword) {
      newErrors.confirmPassword = "Passwords do not match";
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    startTransition(async () => {
      if (!validateForm()) {
        return;
      }

      try {
        const result = await changePassword(currentPassword, newPassword);

        if (result.success) {
          toast.success("Password changed successfully");
          setCurrentPassword("");
          setNewPassword("");
          setConfirmPassword("");
          setErrors({});
        } else {
          toast.error(result.error || "Failed to change password");
        }
      } catch (error) {
        toast.error("Something went wrong");
        console.error(error);
      }
    })
  };

  return (
    <div className="bg-neutral-800/50 rounded-lg p-6">
      <h3 className="text-lg font-semibold mb-4">
        Change Password
      </h3>
      <form
        onSubmit={handleSubmit}
        className="flex flex-col gap-y-2"
      >
        <label className="flex flex-col gap-y-1">
          Current password:
          <Input
            value={currentPassword}
            onChange={(e) => setCurrentPassword(e.target.value)}
            type="password"
            placeholder="Current password"
            disabled={isPending}
            required
          />
          <p className={`text-red-500 text-sm mt-1 ${errors.currentPassword ? "visible" : "invisible"}`}>
            {errors.currentPassword ?? "empty"}
          </p>
        </label>

        <label className="flex flex-col gap-y-1">
          New password:
          <Input
            value={newPassword}
            onChange={(e) => setNewPassword(e.target.value)}
            type="password"
            placeholder="New password"
            disabled={isPending}
            required
          />
          <p className={`text-red-500 text-sm mt-1 ${errors.newPassword ? "visible" : "invisible"}`}>
            {errors.newPassword ?? "empty"}
          </p>
        </label>

        <label className="flex flex-col gap-y-1">
          Repeat new password:
          <Input
            value={confirmPassword}
            onChange={(e) => setConfirmPassword(e.target.value)}
            type="password"
            placeholder="Confirm new password"
            disabled={isPending}
            required
          />
          <p className={`text-red-500 text-sm mt-1 ${errors.confirmPassword ? "visible" : "invisible"}`}>
            {errors.confirmPassword ?? "empty"}
          </p>
        </label>

        <Button type="submit" disabled={isPending}>
          {isPending ? "Changing..." : "Change Password"}
        </Button>
      </form>
    </div>
  );
};

export default ChangePasswordForm;
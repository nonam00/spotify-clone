"use client";

import { type SubmitEvent, useState, useTransition } from "react";
import { useShallow } from "zustand/shallow";
import { z } from "zod";
import { FiCheckCircle } from "react-icons/fi";
import toast from "react-hot-toast";

import { Button, Input } from "@/shared/ui";
import { useAuthStore } from "../model";

const forgotPasswordSchema = z.object({
  email: z.email("Invalid email address")
    .trim()
    .min(1, "Email is required")
    .max(255, "Email must be less than 255 characters"),
});

type ForgotPasswordData = z.infer<typeof forgotPasswordSchema>;

type ForgotPasswordFormProps = {
  onSwitchToLogin: () => void;
}

const ForgotPasswordForm = ({ onSwitchToLogin }: ForgotPasswordFormProps) => {
  const [isPending, startTransition] = useTransition();

  const { forgotPassword, isLoading } = useAuthStore(
    useShallow((s) => ({
      forgotPassword: s.forgotPassword,
      isLoading: s.isLoading,
    }))
  );

  const [formData, setFormData] = useState<ForgotPasswordData>({
    email: ""
  });
  const [showErrors, setShowErrors] = useState<boolean>(false);

  const validate = () => {
    const result = forgotPasswordSchema.safeParse(formData);
    if (result.success) {
      return undefined;
    }
    return z.flattenError(result.error);
  }

  const [emailSent, setEmailSent] = useState(false);

  const onSubmit = async (e: SubmitEvent) => {
    e.preventDefault();
    startTransition(async () => {
      const errors = validate();
      if (errors) {
        setShowErrors(true);
      }

      const { error } = await forgotPassword(formData.email);
      if (error) {
        toast.error(error);
      } else {
        setEmailSent(true);
        toast.success("Password reset instructions sent to your email");
      }
    });
  };

  if (emailSent) {
    return (
      <div className="flex flex-col items-center justify-center gap-y-6 text-center">
        <FiCheckCircle className="w-16 h-16 text-green-500" />
        <div>
          <h3 className="text-xl font-bold text-white mb-2">Check your email</h3>
          <p className="text-neutral-400">
            We&#39;ve sent password reset instructions to your email address.
            Please check your inbox and follow the instructions.
          </p>
        </div>
        <Button
          onClick={onSwitchToLogin}
          className="w-full"
        >
          Return to Login
        </Button>
      </div>
    );
  }

  const errors = showErrors ? validate() : undefined;

  return (
    <form
      onSubmit={onSubmit}
      className="flex flex-col items-center justify-center gap-y-6"
    >
      <label className="flex flex-col gap-y-1 w-full text-base font-bold">
        Email:
        <Input
          value={formData.email}
          onChange={e =>
            setFormData({ ...formData, email: e.currentTarget.value })
          }
          type="email"
          placeholder="Enter your email..."
          disabled={isPending || isLoading}
          required
          maxLength={255}
        />
        <p className={`text-red-500 text-xs mt-1 ${errors?.fieldErrors.email ? "visible" : "invisible"}`}>
          {errors?.fieldErrors.email?.join(", ") ?? "empty"}
        </p>
      </label>

      <div className="flex flex-col gap-y-5 w-full">
        <Button
          type="submit"
          disabled={isPending || isLoading}
          className="w-full"
        >
          {isLoading ? "Sending..." : "Send"}
        </Button>

        <button
          type="button"
          onClick={onSwitchToLogin}
          className="text-sm text-neutral-400 hover:text-green-500 font-medium cursor-pointer"
        >
          ← Back to Login
        </button>
      </div>
    </form>
  );
};

export default ForgotPasswordForm;
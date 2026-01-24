"use client";

import Form from "next/form";
import { useState, useTransition } from "react";
import { useShallow } from "zustand/shallow";
import { FiCheckCircle } from "react-icons/fi";
import toast from "react-hot-toast";

import { Button, Input } from "@/shared/ui";
import { useAuthStore } from "../model";
import { ErrorDisplay } from "../ui";

type ForgotPasswordFormProps = {
  onSwitchToLogin: () => void;
}

const ForgotPasswordForm = ({ onSwitchToLogin }: ForgotPasswordFormProps) => {
  const [isPending, startTransition] = useTransition();

  const { forgotPassword, isLoading, error } = useAuthStore(
    useShallow((s) => ({
      forgotPassword: s.forgotPassword,
      isLoading: s.isLoading,
      error: s.error,
    }))
  );

  const [localError, setLocalError] = useState<string | null>(null);
  const [emailSent, setEmailSent] = useState(false);

  const onSubmit = async (form: FormData) => {
    startTransition(async () => {
      const email = form.get("Email") as string;

      if (!email) {
        setLocalError("Please enter your email");
        return;
      }

      const success = await forgotPassword(email);
      if (success) {
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

  const displayError = error || localError;

  return (
    <Form
      action={onSubmit}
      className="flex flex-col items-center justify-center gap-y-6"
    >
      <div className="flex flex-col gap-y-1 w-full">
        <label className="w-full text-base font-bold">Email:</label>
        <Input
          name="Email"
          type="email"
          placeholder="Enter your email address"
          disabled={isPending || isLoading}
          required
        />
        <p className="text-sm text-neutral-500 mt-2">
          Enter the email address associated with your account and we&#39;ll send you instructions to reset your password.
        </p>
      </div>

      <ErrorDisplay error={displayError} />

      <div className="flex flex-col gap-y-3 w-full">
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
    </Form>
  );
};

export default ForgotPasswordForm;
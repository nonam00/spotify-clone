"use client";

import Form from "next/form";
import { useState, useTransition } from "react";
import { useShallow } from "zustand/shallow";
import toast from "react-hot-toast";

import { Button, Input } from "@/shared/ui";
import { useAuthStore } from "../model";
import { ErrorDisplay } from "../ui";

type RegisterFormProps = {
  onSwitchToLogin: () => void;
}

const RegisterForm = ({ onSwitchToLogin }: RegisterFormProps) => {
  const [isPending, startTransition] = useTransition();

  const { register, isLoading, error } = useAuthStore(
    useShallow((s) => ({
      register: s.register,
      isLoading: s.isLoading,
      error: s.error,
    }))
  );

  const [localError, setLocalError] = useState<string | null>(null);

  const validatePassword = (password: string) => {
    if (password.length < 8) {
      return "Password must be at least 8 characters long";
    }
    return null;
  };

  const onSubmit = async (form: FormData) => {
    startTransition(async () => {
      const email = form.get("Email") as string;
      const password = form.get("Password") as string;
      const fullName = form.get("FullName") as string;

      if (!email || !password || !fullName) {
        setLocalError("Please fill in all fields");
        return;
      }

      const passwordError = validatePassword(password);
      if (passwordError) {
        setLocalError(passwordError);
        return;
      }

      const success = await register(email, password, fullName);
      if (success) {
        toast.success(
          "The confirmation code has been sent to your email. Activate your account and then login."
        );
        onSwitchToLogin();
      }
    });
  };

  const displayError = error || localError;

  return (
    <Form
      action={onSubmit}
      className="flex flex-col items-center justify-center gap-y-4"
    >
      <div className="flex flex-col gap-y-1 w-full">
        <label className="w-full text-base font-bold">Full Name:</label>
        <Input
          name="FullName"
          type="text"
          placeholder="Full Name"
          disabled={isPending || isLoading}
          required
        />
      </div>

      <div className="flex flex-col gap-y-1 w-full">
        <label className="w-full text-base font-bold">Email:</label>
        <Input
          name="Email"
          type="email"
          placeholder="Email"
          disabled={isPending || isLoading}
          required
        />
      </div>

      <div className="flex flex-col gap-y-1 w-full">
        <label className="w-full font-bold">Password:</label>
        <Input
          name="Password"
          type="password"
          placeholder="***********"
          disabled={isPending || isLoading}
          required
          minLength={8}
        />
      </div>

      <ErrorDisplay error={displayError} className="my-1" />

      <div className="flex flex-col gap-y-5 w-full mt-2">
        <Button
          type="submit"
          disabled={isPending || isLoading}
          className="w-full"
        >
          {isLoading ? "Creating account..." : "Create Account"}
        </Button>

        <div className="text-center text-sm text-neutral-400">
          Already have an account?{" "}
          <button
            type="button"
            onClick={onSwitchToLogin}
            className="text-green-500 hover:text-green-600 font-medium cursor-pointer"
          >
            Log in
          </button>
        </div>
      </div>
    </Form>
  );
};

export default RegisterForm;
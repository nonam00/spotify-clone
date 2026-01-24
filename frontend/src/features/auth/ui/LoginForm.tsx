"use client";

import Form from "next/form";
import { useState, useTransition } from "react";
import { useShallow } from "zustand/shallow";
import toast from "react-hot-toast";

import { Button, Input } from "@/shared/ui";
import { useAuthStore } from "../model";
import { ErrorDisplay } from "../ui";

type LoginFormProps = {
  onSwitchToRegister: () => void;
  onForgotPassword: () => void;
}

const LoginForm = ({ onSwitchToRegister, onForgotPassword }: LoginFormProps) => {
  const [isPending, startTransition] = useTransition();

  const { login, isLoading, error } = useAuthStore(
    useShallow((s) => ({
      login: s.login,
      isLoading: s.isLoading,
      error: s.error,
    }))
  );

  const [localError, setLocalError] = useState<string | null>(null);

  const onSubmit = async (form: FormData) => {
    startTransition(async () => {
      const email = form.get("Email") as string;
      const password = form.get("Password") as string;

      if (!email || !password) {
        setLocalError("Please fill in all fields");
        return;
      }

      const success = await login(email, password);
      if (success) {
        toast.success("Logged in successfully");
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
          placeholder="Password"
          disabled={isPending || isLoading}
          required
          minLength={8}
        />
      </div>

      <button
        type="button"
        onClick={onForgotPassword}
        className="text-sm text-neutral-400 hover:text-green-500 self-start my-1 cursor-pointer"
      >
        Forgot your password?
      </button>

      <ErrorDisplay error={displayError} className="my-1" />

      <div className="flex flex-col gap-y-5 w-full">
        <Button
          type="submit"
          disabled={isPending || isLoading}
          className="w-full"
        >
          {isLoading ? "Signing in..." : "Login"}
        </Button>

        <div className="text-center text-sm text-neutral-400">
          Don&#39;t have an account?{" "}
          <button
            type="button"
            onClick={onSwitchToRegister}
            className="text-green-500 hover:text-green-600 font-medium"
          >
            Sign up
          </button>
        </div>
      </div>
    </Form>
  );
};

export default LoginForm;
"use client";

import { type SubmitEvent, useState, useTransition } from "react";
import { useShallow } from "zustand/shallow";
import { z } from "zod";
import toast from "react-hot-toast";

import { Button, Input } from "@/shared/ui";
import { useAuthStore } from "../model";

const loginFormSchema = z.object({
  email: z.email("Invalid email address")
    .trim()
    .min(1, "Email is required")
    .max(255, "Email must be less than 255 characters"),
  password: z.string()
    .trim()
    .min(8, "Password must be at least 8 characters")
    .max(100, "Password must be less than 100 characters"),
});

type LoginFormData = z.infer<typeof loginFormSchema>;

const LoginForm = ({
  onSwitchToRegister,
  onForgotPassword,
}: {
  onSwitchToRegister: () => void;
  onForgotPassword: () => void;
}) => {
  const [isPending, startTransition] = useTransition();

  const { login, isLoading} = useAuthStore(
    useShallow((s) => ({
      login: s.login,
      isLoading: s.isLoading,
    }))
  );

  const [formData, setFormData] = useState<LoginFormData>({
    email: "",
    password: "",
  });
  const [showErrors, setShowErrors] = useState<boolean>(false);

  const validate = () => {
    const result = loginFormSchema.safeParse(formData);
    if (result.success) {
      return undefined;
    }
    return z.flattenError(result.error);
  }

  const onSubmit = async (e: SubmitEvent) => {
    e.preventDefault();

    startTransition(async () => {
      const errors = validate();
      if (errors) {
        console.log(errors);
        setShowErrors(true);
        return;
      }
      console.log(formData);
      const { error } = await login(formData.email, formData.password);
      if (error) {
        toast.error(error);
      } else {
        toast.success("Logged in successfully");
      }
    });
  };

  const errors = showErrors ? validate() : undefined;

  return (
    <form
      onSubmit={onSubmit}
      className="flex flex-col items-center justify-center gap-y-1"
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
      <label className="flex flex-col gap-y-1 w-full font-bold">
        Password:
        <Input
          value={formData.password}
          onChange={e =>
            setFormData({ ...formData, password: e.currentTarget.value })
          }
          type="password"
          placeholder="Enter your password..."
          disabled={isPending || isLoading}
          required
          minLength={8}
          maxLength={255}
        />
        <p className={`text-red-500 text-xs mt-1 ${errors?.fieldErrors.password ? "visible" : "invisible"}`}>
          {errors?.fieldErrors.password?.join(", ") ?? "empty"}
        </p>
      </label>

      <button
        type="button"
        onClick={onForgotPassword}
        className="text-sm text-neutral-400 hover:text-green-500 self-start cursor-pointer mb-5"
      >
        Forgot your password?
      </button>

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
    </form>
  );
};

export default LoginForm;
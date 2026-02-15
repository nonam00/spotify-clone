"use client";

import { type SubmitEvent, useState, useTransition } from "react";
import { useShallow } from "zustand/shallow";
import { z } from "zod";
import toast from "react-hot-toast";

import { Button, Input } from "@/shared/ui";
import { useAuthStore } from "../model";

const registerFormSchema = z.object({
  email: z.email("Invalid email address")
    .trim()
    .min(1, "Email is required")
    .max(255, "Email must be less than 255 characters"),
  password: z.string()
    .trim()
    .min(8, "Password must be at least 8 characters")
    .max(100, "Password must be less than 100 characters"),
  fullName: z.string()
    .trim()
    .min(1, "Full name is required")
    .max(255, "Full name must be less than 100 characters"),
});

type RegisterFormData = z.infer<typeof registerFormSchema>;

const RegisterForm = ({
  onSwitchToLogin
}: {
  onSwitchToLogin: () => void;
}) => {
  const [isPending, startTransition] = useTransition();

  const { register, isLoading } = useAuthStore(
    useShallow((s) => ({
      register: s.register,
      isLoading: s.isLoading,
    }))
  );

  const [formData, setFormData] = useState<RegisterFormData>({
    email: "",
    password: "",
    fullName: "",
  });
  const [showErrors, setShowErrors] = useState<boolean>(false);

  const validate = () => {
    const result = registerFormSchema.safeParse(formData);
    if (result.success) {
      return undefined;
    }
    return z.flattenError(result.error);
  }

  const onSubmit = async (e: SubmitEvent) => {
    console.log(e);
    e.preventDefault();

    startTransition(async () => {
      const errors = validate();
      if (errors) {
        setShowErrors(true);
        return;
      }

      const { error } = await register(formData.email, formData.password, formData.fullName);
      if (error) {
        toast.error(error);
      } else {
        toast.success(
          "The confirmation code has been sent to your email. Activate your account and then login."
        );
        onSwitchToLogin();
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
        Full name:
        <Input
          value={formData.fullName}
          onChange={e =>
            setFormData({ ...formData, fullName: e.currentTarget.value })
          }
          placeholder="Enter your full name..."
          disabled={isPending || isLoading}
          required
          maxLength={255}
        />
        <p className={`text-red-500 text-xs mt-1 ${errors?.fieldErrors.fullName ? "visible" : "invisible"}`}>
          {errors?.fieldErrors.fullName?.join(", ") ?? "empty"}
        </p>
      </label>
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
    </form>
  );
};

export default RegisterForm;
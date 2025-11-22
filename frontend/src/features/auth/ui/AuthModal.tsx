"use client";

import { useRouter } from "next/navigation";
import Form from "next/form";
import { useLayoutEffect, useState, useTransition } from "react";
import { useShallow } from "zustand/shallow";
import toast from "react-hot-toast";

import { Button, Input, Modal } from "@/shared/ui";
import { useAuthStore, useAuthModalStore } from "../model";

type AuthSubmitType = "login" | "register";

const AuthModal = () => {
  const router = useRouter();
  const [onClose, isOpen] = useAuthModalStore(
    useShallow((s) => [s.onClose, s.isOpen])
  );
  const { isAuthenticated, login, register, isLoading, error } = useAuthStore();

  const [submitType, setSubmitType] = useState<AuthSubmitType>();
  const [isPending, startTransition] = useTransition();

  useLayoutEffect(() => {
    if (isAuthenticated) {
      router.refresh();
      onClose();
    }
  }, [isAuthenticated, router, onClose]);

  const onChange = (open: boolean) => {
    if (!open) {
      onClose();
    }
  };

  const onSubmit = async (form: FormData) => {
    startTransition(async () => {
      const email = form.get("Email") as string;
      const password = form.get("Password") as string;

      if (submitType === "login") {
        const success = await login(email, password);
        if (success) {
          toast.success("Logged in");
        }
      } else if (submitType === "register") {
        const success = await register(email, password);
        if (success) {
          toast.success(
            "The confirmation code has been sent to your email. Activate your account and then login."
          );
        }
      }
    });
  };

  return (
    <Modal
      title="Welcome back"
      description="Log in into your account"
      isOpen={isOpen}
      onChange={onChange}
    >
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
        <div
          className={`
            w-full text-red-400 text-sm text-center bg-red-500/10 border border-red-500/30 rounded-md py-1 px-3
            ${error ? "visible" : "invisible"} 
          `}
        >
          {error ?? "no errors"}
        </div>
        <div className="flex flex-col gap-y-3 w-full">
          <Button
            onClick={() => setSubmitType("login")}
            type="submit"
            disabled={isPending || isLoading}
          >
            {isLoading ? "Signing in..." : "Login"}
          </Button>
          <Button
            onClick={() => setSubmitType("register")}
            type="submit"
            disabled={isPending || isLoading}
            className="bg-transparent hover:bg-neutral-700 text-neutral-300 font-medium"
          >
            Register
          </Button>
        </div>
      </Form>
    </Modal>
  );
};

export default AuthModal;


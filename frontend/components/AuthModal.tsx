"use client";

import { useRouter } from "next/navigation";
import Form from "next/form";
import {useLayoutEffect, useState, useTransition} from "react";
import { useShallow } from "zustand/shallow";

import useAuthModal from "@/hooks/useAuthModal";
import { useUser } from "@/hooks/useUser";

import {AuthSubmitType} from "@/types/types";

import Button from "@/components/ui/Button";
import Input from "@/components/ui/Input";
import Modal from "@/components/ui/Modal";

const AuthModal = () => {
  const router = useRouter();
  const [onClose, isOpen] = useAuthModal(useShallow(s => [s.onClose, s.isOpen]));
  const { isAuth, authorize } = useUser();
  const [submitType, setSubmitType] = useState<AuthSubmitType>();
  const [isPending, startTransition] = useTransition();

  useLayoutEffect(() => {
    if (isAuth) {
      router.refresh();
      onClose();
    }
  }, [isAuth, router, onClose]);

  const onChange = (open: boolean) => {
    if (!open) {
      onClose();
    }
  }

  const onSubmit = async (form: FormData) => {
    startTransition(async () => {
      const success = await authorize(submitType!, form);
      if (success) {
        onClose();
        router.refresh();
      }
    })
  }
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
          <label className="w-full text-base font-bold">
            Email:
          </label>
          <Input
            name="Email"
            type="email"
            placeholder="Email"
            disabled={isPending}
            required
          />
        </div>
        <div className="flex flex-col gap-y-1 w-full">
          <label className="w-full font-bold">
            Password:
          </label>
          <Input
            name="Password"
            type="password"
            placeholder="Password"
            disabled={isPending}
            required
            minLength={8}
          />
        </div>
        <div className="flex flex-col gap-y-3 w-full mt-5">
          <Button
            onClick={() => setSubmitType("login")}
            type="submit"
            disabled={isPending}
          >
            Login
          </Button>
          <Button
            onClick={() => setSubmitType("register")}
            type="submit"
            disabled={isPending}
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


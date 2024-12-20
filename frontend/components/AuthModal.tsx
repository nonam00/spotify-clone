"use client";

import { useRouter } from "next/navigation";
import Form from "next/form";
import { useEffect, useState } from "react";
import { useShallow } from "zustand/shallow";

import useAuthModal from "@/hooks/useAuthModal";
import { useUser } from "@/hooks/useUser";

import Modal from "./Modal";
import Input from "./Input";
import Button from "./Button";

import login from "@/services/auth/login";
import register from "@/services/auth/register";

type SubmitType = "login" | "register";

const actions = {
  "login": login,
  "register": register
};

const AuthModal = () => {
  const router = useRouter();
  const [onClose, isOpen] = useAuthModal(useShallow(s => [s.onClose, s.isOpen]));
  const { isAuth, authorize } = useUser();
  const [submitType, setSubmitType] = useState<SubmitType>();

  useEffect(() => {
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
    await authorize(actions[submitType!], form);
    if (isAuth) {
      onClose();
      router.refresh();
    }
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
        className="
          flex
          flex-col
          items-center
          justify-center
        "
      >
        <Input
          name="Email"
          type="email"
          placeholder="Email"
          className="my-3 py-2 text-base"
          required
        />
        <Input
          name="Password"
          type="password"
          placeholder="Password"
          className="text-base py-2"
          required
          minLength={8}
        />
        <Button
          onClick={() => setSubmitType("login")}
          className="mt-10 mb-3"
          type="submit"
        >
          Login
        </Button>
        <Button
          onClick={() => setSubmitType("register")}
          className="
            my-2
            hover:bg-neutral-700
            bg-transparent
            text-neutral-300
            font-medium
          "
          type="submit"
        >
          Register
        </Button>
      </Form>
    </Modal>
  );
};

export default AuthModal;


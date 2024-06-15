"use client";

import { useRouter } from "next/navigation";
import { useEffect, useState } from "react";
import toast from "react-hot-toast";

import useAuthModal from "@/hooks/useAuthModal";
import { useUser } from "@/hooks/useUser";

import Button from "./Button";
import Modal from "./Modal";

const AuthModal = () => {
  const router = useRouter();
  const { onClose, isOpen } = useAuthModal();
  const user = useUser();
  const [email, setEmail] = useState<string>("");
  const [password, setPassword] = useState<string>("");  

  useEffect(() => {
    if(user.isAuth) {
      router.refresh();
      onClose();
    }
  }, [user.isAuth, router, onClose]);


  const onChange = (open: boolean) => {
    if(!open) {
      onClose();
    }
  }

  const upload = async (func: any) => {
    if(!email) {
      toast.error("The email field must be filled in");
      return;
    }
    if (!password) {
      toast.error("The password field must be filled in");
      return;
    }
    await func(email, password);
  }
  
  return (
    <Modal
      title="Welcome back"
      description="Log in into your account"
      isOpen={isOpen}
      onChange={onChange}
    >
      <div className="
        flex
        flex-col
        items-center
        justify-center
      ">
        <input
          onChange={(e) => setEmail(e.target.value)}
          value={email}
          type="text"
          placeholder="email"
          className="
            my-3
            p-2
            w-full
            rounded-md
          "
        />
        <input
          onChange={(e) => setPassword(e.target.value)}
          value={password}
          type="password"
          placeholder="password"
          className="
            my-3
            p-2
            w-full
            rounded-md
          "
        />
        <Button
          onClick={async () => await upload(user.login)}
          className="mt-7 mb-3"
        >
          Login
        </Button>
        <Button
          onClick={async () => await upload(user.register)}
          className="
            my-3
            hover:bg-neutral-700
            bg-transparent
            text-neutral-300
            font-medium
          " 
        >
          Register
        </Button>
      </div>
    </Modal>
  );
};

export default AuthModal;

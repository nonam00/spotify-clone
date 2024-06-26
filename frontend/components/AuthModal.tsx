"use client";

import { useRouter } from "next/navigation";
import { useEffect, useState } from "react";
import toast from "react-hot-toast";

import useAuthModal from "@/hooks/useAuthModal";
import { useUser } from "@/hooks/useUser";

import Modal from "./Modal";
import Input from "./Input";
import Button from "./Button";

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
    if (!email) {
      toast.error("The email field must be filled in");
      return;
    }
    if (!/^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$/.test(email)) {
      toast.error("Non valid email");
      return;
    }
    if (!password) {
      toast.error("The password field must be filled in");
      return;
    }
    if (password.length < 8) {
      toast.error("The password length must be greater than 8");
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
        <Input
          onChange={(e) => setEmail(e.target.value)}
          value={email}
          type="email"
          placeholder="Email"
          className="my-3 py-2 text-base"
        />
        <Input
          onChange={(e) => setPassword(e.target.value)}
          value={password}
          type="password"
          placeholder="Password"
          className="text-base py-2"
        />
        <Button
          onClick={async () => await upload(user.login)}
          className="mt-10 mb-3"
        >
          Login
        </Button>
        <Button
          onClick={async () => await upload(user.register)}
          className="
            my-2
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

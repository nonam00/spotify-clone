"use client";

import { useRouter } from "next/navigation";
import { useEffect, useState } from "react";

import useAuthModal from "@/hooks/useAuthModal";
import { useUser } from "@/hooks/useUser";
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
  
  // TODO: styles for auth form
  return (
    <Modal
      title="Welcome back"
      description="Log in into your account"
      isOpen={isOpen}
      onChange={onChange}
    >
      <input
        onChange={(e) => setEmail(e.target.value)}
        value={email}
        type="text"
        placeholder="email"
      />
      <input
        onChange={(e) => setPassword(e.target.value)}
        value={password}
        type="password"
        placeholder="password"
      />
      <button
        onClick={async () => await user.login(email, password)}
      >
        Login
      </button>
      <button onClick={async () => await user.register(email, password)}>
        Register
      </button>
    </Modal>
  );
};

export default AuthModal;
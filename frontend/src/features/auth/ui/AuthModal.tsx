"use client";

import { useRouter } from "next/navigation";
import { useLayoutEffect } from "react";
import { useShallow } from "zustand/shallow";

import { Modal } from "@/shared/ui";
import { useAuthStore, useAuthModalStore } from "../model";
import { LoginForm, RegisterForm, ForgotPasswordForm } from "../ui";

const AuthModal = () => {
  const router = useRouter();
  const [isOpen, onClose, currentView, setView] = useAuthModalStore(
    useShallow((s) => [s.isOpen, s.onClose, s.currentView, s.setView])
  );
  const { isAuthenticated, cleanError } = useAuthStore();

  useLayoutEffect(() => {
    if (isOpen && isAuthenticated) {
      router.refresh();
      onClose();
      cleanError();
    }
  }, [isAuthenticated, router, onClose, cleanError, isOpen]);

  const onChange = (open: boolean) => {
    if (!open) {
      cleanError();
      onClose();
    }
  };

  const changeView = (view: "login" | "register" | "forgot-password") => {
    cleanError();
    setView(view);
  }

  const getModalConfig = () => {
    switch (currentView) {
      case 'login':
        return {
          title: "Welcome back",
          description: "Log in to your account"
        };
      case 'register':
        return {
          title: "Create an account",
          description: "Sign up to get started"
        };
      case 'forgot-password':
        return {
          title: "Reset your password",
          description: "Enter your email to receive reset instructions"
        };
    }
  };

  const { title, description } = getModalConfig();

  return (
    <Modal
      title={title}
      description={description}
      isOpen={isOpen}
      onChange={onChange}
    >
      {currentView === 'login' &&
        <LoginForm
          onForgotPassword={() => changeView("forgot-password")}
          onSwitchToRegister={() => changeView("register")}
        />
      }
      {currentView === "register" && <RegisterForm onSwitchToLogin={() => changeView("login")} />}
      {currentView === "forgot-password" && <ForgotPasswordForm onSwitchToLogin={() => changeView("login")} />}
    </Modal>
  );
};

export default AuthModal;
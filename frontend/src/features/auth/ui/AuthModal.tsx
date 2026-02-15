"use client";

import { useRouter } from "next/navigation";
import {memo, useCallback, useLayoutEffect} from "react";
import { useShallow } from "zustand/shallow";

import { Modal } from "@/shared/ui";
import { useAuthStore, useAuthModalStore, AuthView } from "../model";
import { LoginForm, RegisterForm, ForgotPasswordForm } from "../ui";

type ViewInfo = {
  title: string;
  description: string;
}

const authViewInfo: Record<AuthView, ViewInfo> = {
  "login": {
    title: "Welcome back",
    description: "Log in to your account",
  },
  "register": {
    title: "Create an account",
    description: "Sign up to get started",
  },
  "forgot-password": {
    title: "Reset your password",
    description: "Enter the email address associated with your account and we'll send you instructions to reset your password"
  }
};

const AuthModal = memo(function AuthModal() {
  const router = useRouter();

  const { isOpen, onClose, currentView, setView } = useAuthModalStore(
    useShallow((s) => ({
      isOpen: s.isOpen,
      onClose: s.onClose,
      currentView: s.currentView,
      setView: s.setView
    }))
  );

  const { isAuthenticated, cleanError } = useAuthStore(
    useShallow((s) => ({
      isAuthenticated: s.isAuthenticated,
      cleanError: s.cleanError
    }))
  );

  useLayoutEffect(() => {
    if (isOpen && isAuthenticated) {
      router.refresh();
      onClose();
      cleanError();
    }
  }, [isAuthenticated, router, onClose, cleanError, isOpen]);

  const onChange = useCallback((open: boolean) => {
    if (!open) {
      cleanError();
      onClose();
    }
  }, [cleanError, onClose]);

  const changeView = useCallback((view: AuthView) => {
    cleanError();
    setView(view);
  }, [setView, cleanError]);

  return (
    <Modal
      title={authViewInfo[currentView].title}
      description={authViewInfo[currentView].description}
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
});

export default AuthModal;
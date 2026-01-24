"use client";

import { useEffect } from "react";
import {useShallow} from "zustand/shallow";
import { useAuthStore } from "@/features/auth";

const AuthProvider = ({
  children
}: {
  children: React.ReactNode;
}) => {
  const { checkAuth, isAuthenticated } = useAuthStore(
    useShallow((s) => ({
      checkAuth: s.checkAuth,
      isAuthenticated: s.isAuthenticated,
    }))
  );

  useEffect(() => {
    if (!isAuthenticated) {
      void checkAuth();
    }
  }, [isAuthenticated, checkAuth]);

  return <>{children}</>;
};

export default AuthProvider;
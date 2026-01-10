"use client";

import { useEffect } from "react";
import { useAuthStore } from "@/features/auth";

const AuthProvider = ({
  children
}: {
  children: React.ReactNode;
}) => {
  const { checkAuth, isAuthenticated } = useAuthStore();

  useEffect(() => {
    if (!isAuthenticated) {
      void checkAuth();
    }
  }, [isAuthenticated, checkAuth]);

  return <>{children}</>;
};

export default AuthProvider;
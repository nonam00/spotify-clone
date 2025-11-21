"use client";

import { useEffect } from "react";
import { useAuthStore } from "@/features/auth";

type AuthProviderProps = {
  children: React.ReactNode;
}

const AuthProvider: React.FC<AuthProviderProps> = ({ children }) => {
  const { checkAuth, isAuthenticated } = useAuthStore();

  useEffect(() => {
    if (!isAuthenticated) {
      void checkAuth();
    }
  }, [isAuthenticated, checkAuth]);

  return <>{children}</>;
};

export default AuthProvider;
import { useEffect } from "react";
import { Navigate } from "react-router-dom";

import { Box } from "@/shared/ui";
import type { ModeratorPermissions } from "@/entities/moderator";
import { useAuthStore } from "../model";
import {useShallow} from "zustand/react/shallow";

interface ProtectedRouteProps {
  children: React.ReactNode;
  requiredPermission?: keyof ModeratorPermissions;
}

const sections = [
  { permission: "canManageContent" as const, path: "/" },
  { permission: "canManageModerators" as const, path: "/moderators" },
  { permission: "canManageUsers" as const, path: "/users" },
];

const ProtectedRoute = ({ children, requiredPermission }: ProtectedRouteProps) => {
  const { isLoading, isAuthenticated, checkAuth, user } = useAuthStore(
    useShallow((s) => ({
      isLoading: s.isLoading,
      isAuthenticated: s.isAuthenticated,
      checkAuth: s.checkAuth,
      user: s.user,
    }))
  );

  useEffect(() => {
    if (!isAuthenticated) {
      void checkAuth();
    }
  }, [isAuthenticated, checkAuth]);

  if (isLoading) {
    return (
      <div className="h-full flex items-center justify-center bg-black">
        <Box className="p-12">
          <div className="flex flex-col items-center justify-center gap-4">
            <div className="animate-spin rounded-full h-12 w-12 border-4 border-green-500 border-t-transparent"></div>
            <p className="text-white text-lg font-medium">Checking access…</p>
          </div>
        </Box>
      </div>
    );
  }

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  if (requiredPermission && !user?.permissions?.[requiredPermission]) {
    const fallbackRoute = user
      ? sections.find(
          (section) => section.permission !== requiredPermission && user.permissions?.[section.permission]
        )?.path
      : null;

    if (fallbackRoute) {
      return <Navigate to={fallbackRoute} replace />;
    }

    return (
      <div className="h-full flex items-center justify-center bg-black p-6">
        <Box className="max-w-lg p-10 text-center space-y-4">
          <h2 className="text-2xl font-semibold text-white">Access denied</h2>
          <p className="text-neutral-300 text-sm">
            You don&apos;t have enough permissions to open this section. Please contact a super moderator.
          </p>
        </Box>
      </div>
    );
  }

  return <>{children}</>;
};

export default ProtectedRoute;
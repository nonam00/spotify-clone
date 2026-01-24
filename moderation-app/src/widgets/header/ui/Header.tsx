import { useCallback } from "react";
import { useNavigate } from "react-router-dom";
import { useShallow } from "zustand/react/shallow";
import { twMerge } from "tailwind-merge";

import { useAuthStore } from "@/features/auth";
import { Button } from "@/shared/ui";

interface HeaderProps {
  title: string;
  description?: string;
  children?: React.ReactNode;
  className?: string;
}

const Header = ({ title, description, children, className }: Readonly<HeaderProps>) => {
  const navigate = useNavigate();

  const { user, logout } = useAuthStore(
    useShallow((s) => ({
      user: s.user,
      logout: s.logout,
    }))
  );

  const handleLogout = useCallback(async () => {
    await logout();
    navigate("/login");
  }, [logout, navigate]);

  return (
    <div
      className={twMerge(`h-fit bg-linear-to-b from-emerald-800 p-8 shadow-2xl`, className)}
    >
      <div className="w-full mb-4 flex items-center justify-between flex-wrap gap-4">
        <div>
          <h1 className="text-4xl font-bold text-white mb-2">{title}</h1>
          {description && <p className="text-neutral-200 text-base">{description}</p>}
        </div>
        {user && (
          <div className="flex items-center gap-4">
            <div className="text-right">
              <p className="text-white font-medium">{user.fullName || "Moderator"}</p>
              <p className="text-neutral-300 text-sm">{user.email}</p>
            </div>
            <Button onClick={handleLogout} className="bg-white hover:bg-neutral-200 text-black px-4 py-2 text-sm">
              Logout
            </Button>
          </div>
        )}
      </div>
      {children}
    </div>
  );
};

export default Header;


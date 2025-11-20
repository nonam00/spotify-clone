import { useNavigate } from "react-router-dom";
import { twMerge } from "tailwind-merge";

import { useAuthStore } from "@/shared/store/authStore";
import { Button } from "@/shared/ui";

interface HeaderProps {
  title: string;
  description?: string;
  children?: React.ReactNode;
  className?: string;
}

const Header = ({ title, description, children, className }: Readonly<HeaderProps>) => {
  const navigate = useNavigate();
  const { user, logout } = useAuthStore();

  const handleLogout = async () => {
    await logout();
    navigate("/login");
  };

  return (
    <div
      className={twMerge(`h-fit bg-gradient-to-b from-emerald-800 p-8 shadow-2xl`, className)}
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


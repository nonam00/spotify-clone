"use client";

import Image from "next/image";
import { useRouter } from "next/navigation";
import { useCallback } from "react";
import {useShallow} from "zustand/shallow";
import { twMerge } from "tailwind-merge";
import toast from "react-hot-toast";
import { HiHome, HiSearch } from "react-icons/hi";
import { IoMdPerson } from "react-icons/io";
import { RxCaretLeft, RxCaretRight } from "react-icons/rx";

import { Button } from "@/shared/ui";
import { CLIENT_FILES_URL } from "@/shared/config/api";
import { useAuthStore, useAuthModalStore } from "@/features/auth";
import { usePlayerStore } from "@/widgets/player";

const Header = ({
  children,
  className,
}: Readonly<{
  children: React.ReactNode;
  className?: string;
}>) => {
  const router = useRouter();

  const { openAuthModal, setAuthView } = useAuthModalStore(
    useShallow((s) => ({
      openAuthModal: s.onOpen,
      setAuthView: s.setView,
    }))
  );

  const { isAuthenticated, user, logout } = useAuthStore(
    useShallow((s) => ({
      isAuthenticated: s.isAuthenticated,
      user: s.user,
      logout: s.logout,
    })),
  );

  const resetPlayer = usePlayerStore(s => s.reset);

  const openLoginModal = useCallback(() => {
    setAuthView("login");
    openAuthModal();
  }, [openAuthModal, setAuthView]);

  const openRegisterModal = useCallback(() => {
    setAuthView("register");
    openAuthModal();
  }, [openAuthModal, setAuthView]);

  const handleLogout = async () => {
    try {
      await logout();
      resetPlayer();
      router.refresh();

      toast.error("Logged out");
    } catch (error) {
      toast.error((error as Error)?.message || "Failed to logout");
    }
  };

  return (
    <div className={twMerge(`h-fit bg-linear-to-b from-emerald-800 p-6`, className)}>
      <div className="w-full mb-4 flex items-center justify-between">
        <div className="hidden md:flex gap-x-2 items-center">
          <button
            onClick={() => router.back()}
            className="flex items-center justify-center rounded-full bg-black hover:opacity-75 transitition"
          >
            <RxCaretLeft className="text-white" size={35} />
          </button>
          <button
            onClick={() => router.forward()}
            className="flex items-center justify-center rounded-full bg-black hover:opacity-75 transitition"
          >
            <RxCaretRight className="text-white" size={35} />
          </button>
        </div>
        <div className="flex md:hidden gap-x-2 items-center">
          <button
            onClick={() => router.push("/")}
            className="flex items-center justify-center rounded-full p-2 bg-white hover:opacity-75 transition"
          >
            <HiHome className="text-black" size={20} />
          </button>
          <button
            onClick={() => router.push("/search")}
            className="flex items-center justify-center rounded-full p-2 bg-white hover:opacity-75 transition"
          >
            <HiSearch className="text-black" size={20} />
          </button>
        </div>
        {isAuthenticated ? (
          <div className="flex items-center gap-x-4 h-11">
            <Button onClick={handleLogout} className="bg-white px-6 py-2">Logout</Button>
            <Button onClick={() => router.push("/account")} className="bg-white p-0">
              {user?.avatarPath ? (
                <div className="relative w-11 h-11 rounded-full overflow-hidden">
                  <Image
                    fill
                    src={`${CLIENT_FILES_URL}/download-url?type=image&file_id=${user.avatarPath}`}
                    alt="Avatar"
                    className="object-cover"
                    loading="lazy"
                    unoptimized
                  />
                </div>
              ) : (
                <div className="w-11 h-11 bg-white rounded-full flex items-center justify-center">
                  <IoMdPerson className="w-6 h-6 text-black" />
                </div>
              )}
            </Button>
          </div>
        ) : (
          <div className="flex items-center gap-x-4 h-11">
            <Button
              onClick={openRegisterModal}
              className="bg-transparent text-neutral-300 font-medium whitespace-nowrap px-6 py-2"
            >
              Sign Up
            </Button>
            <Button
              onClick={openLoginModal}
              className="bg-white whitespace-nowrap px-6 py-2"
            >
              Log In
            </Button>
          </div>
        )}
      </div>
      {children}
    </div>
  );
};

export default Header;

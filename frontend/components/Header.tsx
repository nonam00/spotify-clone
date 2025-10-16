"use client";

import Image from "next/image";
import { useRouter } from "next/navigation";
import { twMerge } from "tailwind-merge";
import toast from "react-hot-toast";
import { HiHome, HiSearch } from "react-icons/hi";
import {IoMdPerson} from "react-icons/io";
import { RxCaretLeft, RxCaretRight } from "react-icons/rx";

import useAuthModal from "@/hooks/useAuthModal";
import { useUser } from "@/hooks/useUser";
import Button from "@/components/ui/Button";
import {CLIENT_API_URL, CLIENT_FILES_URL} from "@/helpers/api";

const Header = ({
  children,
  className
}: Readonly<{
  children: React.ReactNode;
  className?: string
}>) => {
  const openAuthModal = useAuthModal(s => s.onOpen);
  const router = useRouter();
  const user = useUser();

  const handleLogout = async () => {
    try {
      await user.logout();
      router.refresh();
      toast.error('Logged out');
    } catch(error) {
      toast.error(error?.message);
    }
  };

  return (
    <div className={twMerge(`h-fit bg-gradient-to-b from-emerald-800 p-6`, className)}>
      <div className="w-full mb-4 flex items-center justify-between">
        <div className="hidden md:flex gap-x-2 items-center">
          <button
            onClick={() => router.back()}
            className="flex items-center justify-center rounded-full bg-black hover:opacity-75 transitition"
          >
            <RxCaretLeft className="text-white" size={35}/>
          </button>
          <button
            onClick={() => router.forward()}
            className="flex items-center justify-center rounded-full bg-black hover:opacity-75 transitition"
          >
            <RxCaretRight className="text-white" size={35}/>
          </button>
        </div>
        <div className="flex md:hidden gap-x-2 items-center">
          <button
            onClick={() => {router.push('/')}}
            className="flex items-center justify-center rounded-full p-2 bg-white hover:opacity-75 transition"
          >
            <HiHome className="text-black" size={20}/>
          </button>
          <button
            onClick={() => {router.push('/search')}}
            className="flex items-center justify-center rounded-full p-2 bg-white hover:opacity-75 transition"
          >
            <HiSearch className="text-black" size={20}/>
          </button>
        </div>
        <div className="flex justify-between items-center gap-x-4">
          {user.isAuth ? (
            <div className="flex gap-x-4 items-center">
              <Button
                onClick={handleLogout}
                className="bg-white px-6 py-2"
              >
                Logout
              </Button>
              <Button
                onClick={() => router.push('/account')}
                className="bg-white p-0"
              >
                {user.userDetails?.avatarPath ? (
                  <Image
                    src={`${CLIENT_FILES_URL}/download-url?type=image&file_id=${user.userDetails.avatarPath}`}
                    alt="Avatar"
                    className="w-11 h-11 rounded-full object-cover"
                    loading="lazy"
                    unoptimized
                  />
                ) : (
                  <div className="w-11 h-11 bg-white rounded-full flex items-center justify-center">
                    <IoMdPerson className="w-6 h-6 text-black" />
                  </div>
                )}
              </Button>
            </div>
          ) : (
            <>
              <div>
                <Button
                  onClick={openAuthModal}
                  className="bg-transparent text-neutral-300  font-medium"
                >
                  Sign Up
                </Button>
              </div>
              <div>
                <Button
                  onClick={openAuthModal}
                  className="bg-white px-6 py-2"
                >
                  Log In
                </Button>
              </div>
            </>
          )}
        </div>
      </div>
      {children}
    </div>
  );
}
 
export default Header;

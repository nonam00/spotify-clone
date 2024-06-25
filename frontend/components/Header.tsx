"use client";

import { useRouter } from "next/navigation";
import { HiHome, HiSearch } from "react-icons/hi";
import { RxCaretLeft, RxCaretRight } from "react-icons/rx";
import { FaUserAlt } from "react-icons/fa";
import { twMerge } from "tailwind-merge";

import Button from "./Button";
import useAuthModal from "@/hooks/useAuthModal";
import { useUser } from "@/hooks/useUser";
import toast from "react-hot-toast";

interface HeaderProps {
  children: React.ReactNode;
  className?: string
}

const Header: React.FC<HeaderProps> = ({
  children,
  className
}) => {
  const authModal = useAuthModal();
  const router = useRouter();

  const user = useUser();

  const handleLogout = async () => {
    try {
      await user.logout();
      router.refresh();
      toast.error('Logged out');
    } catch(error: any) {
      toast.error(error?.message);
    }
  };

  return (
    <div
      className={twMerge(`
        h-fit
        bg-gradient-to-b
        from-emerald-800
        p-6
      `,
        className
      )}
    >
      <div className="
        w-full
        mb-4
        flex
        items-center
        justify-between
      ">
        <div className="
          hidden 
          md:flex 
          gap-x-2 
          items-center
        ">
          <button
            onClick={() => router.back()}
            className="
              rounded-full
              bg-black
              flex
              items-center
              justify-center
              hover:opacity-75
              transitition
            "
          >
            <RxCaretLeft className="text-white" size={35}/>
          </button>
          <button
            onClick={() => router.forward()}
            className="
              rounded-full
              bg-black
              flex
              items-center
              justify-center
              hover:opacity-75
              transitition
            "
          >
            <RxCaretRight className="text-white" size={35}/>
          </button>
        </div>
        <div className="flex md:hidden gap-x-2 items-center">
          <button
            onClick={() => {router.push('/')}}
            className="
              rounded-full
              p-2
              bg-white
              flex
              items-center
              justify-center
              hover:opacity-75
              transition
            "
          >
            <HiHome className="text-black" size={20}/>
          </button>
          <button
            onClick={() => {router.push('/search')}}
            className="
              rounded-full
              p-2
              bg-white
              flex
              items-center
              justify-center
              hover:opacity-75
              transition
            "
          >
            <HiSearch className="text-black" size={20}/>
          </button>
        </div>
        <div
          className="
            flex
            justify-between
            items-center
            gap-x-4
          "
        >
          {user.isAuth ? (
            <div className="flex gap-x-4 items-center">
              <Button
                onClick={handleLogout}
                className="bg-white px-6 py-2"
              >
                Logout
              </Button>
              <Button
                // TODO: replace with actual account details page
                onClick={() => {}} 
                className="bg-white"
              >
                <FaUserAlt />
              </Button>
            </div>
          ) : (
            <>
              <div>
                <Button
                  onClick={authModal.onOpen}
                  className="
                    bg-transparent
                    text-neutral-300
                    font-medium
                  "
                >
                  Sign Up
                </Button>
              </div>
              <div>
                <Button
                  onClick={authModal.onOpen}
                  className="
                    bg-white
                    px-6
                    py-2
                  "
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

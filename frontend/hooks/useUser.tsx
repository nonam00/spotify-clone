import {createContext, useContext, useEffect, useState, useTransition} from "react";
import toast from "react-hot-toast";

import {UserDetails} from "@/types/types";
import {authFetchClient, CLIENT_API_URL} from "@/helpers/api";

type UserContextType = {
  isAuth: boolean;
  userDetails: UserDetails | null;
  isLoading: boolean;
  login: (form: FormData) => Promise<void>;
  register: (form: FormData) => Promise<void>;
  logout: () => Promise<void>;
}

export const UserContext = createContext<UserContextType | undefined>(undefined);

export const MyUserContextProvider = ({
  children
}: {
  children: React.ReactNode
}) => {
  const [isAuth, setIsAuth] = useState<boolean>(false);
  const [isLoadingData, startTransition] = useTransition();
  const [userDetails, setUserDetails] = useState<UserDetails | null>(null);

  async function login(form: FormData) {
    startTransition(async () => {
      const response = await fetch(`${CLIENT_API_URL}/auth/login/`, {
        method: "POST",
        credentials: "include",
        body: form
      });
      if (!response.ok) {
        if (response.status === 400) {
          const error = await response.json();
          toast.error(error.detail);
        } else {
          toast.error("An error occurred when you tried to log in.")
        }
      } else {
        setIsAuth(true);
        toast.success("Logged in");
      }
    })
  }
  
  async function register(form: FormData) {
    startTransition(async () => {
      const response = await fetch(`${CLIENT_API_URL}/auth/register/`, {
        method: "POST",
        credentials: "include",
        body: form
      });
      if (!response.ok) {
        if (response.status === 400) {
          const error = await response.json();
          toast.error(error.detail);
        } else {
          toast.error("An error occurred when you tried to register your account.")

        }
      } else {
        toast.success("The confirmation code has been sent to your email. Activate your account and then login.")
      }
    })
  }

  async function logout() {
    const response = await fetch(`${CLIENT_API_URL}/auth/logout/`, {
      method: "POST",
      credentials: "include",
    });
    if (response.ok) {
      setIsAuth(false);
      setUserDetails(null);
    }
  }

  useEffect(() => {
    if (!isAuth) {
      startTransition(async () => {
        const response = await authFetchClient(`${CLIENT_API_URL}/users/info`, {
          headers: {
            "Content-Type": "application/json",
          },
          method: "GET",
          credentials: "include"
        });
        if (response.ok) {
          setIsAuth(true);
          const user = await response.json();
          setUserDetails(user);
          toast.success("Logged in");
        }
      });
    }
  }, [isAuth]);

  const value = {
    isAuth,
    userDetails,
    isLoading: isLoadingData,
    login,
    register,
    logout
  };

  return <UserContext.Provider value={value}>{children}</UserContext.Provider>;
};

export const useUser = () => {
  const context = useContext(UserContext);
  if (context === undefined) {
    throw new Error(`useUser must be used within a MyUserContextProvider.`);
  }
  return context;
};

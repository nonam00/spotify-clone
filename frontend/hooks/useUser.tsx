import { createContext, useContext, useEffect, useState } from "react";
import toast from "react-hot-toast";

import { UserDetails } from "@/types/types";
import loginRequest from "@/services/auth/login";
import getUserInfo from "@/services/auth/getUserInfo";
import registerRequest from "@/services/auth/register";
import logoutRequest from "@/services/auth/logout";

type UserContextType = {
  isAuth: boolean;
  userDetails: UserDetails | null;
  isLoading: boolean;
  login: (email: string, password: string) => Promise<void>;
  register: (email: string, password: string) => Promise<void>;
  logout: () => Promise<void>;
}

export const UserContext = createContext<UserContextType | undefined>(
  undefined
);

export interface Props {
  [propName: string]: any;
}

export const MyUserContextProvider = (props: Props) => {
  const [isAuth, setIsAuth] = useState<boolean>(false);
  const [isLoadingData, setIsLoadingData] = useState<boolean>(false);
  const [userDetails, setUserDetails] = useState<UserDetails | null>(null);

  const login = async (email: string, password: string) => {
    const response = await loginRequest(email, password);

    if (response.ok) {
      await getInfo();
    } else {
      const data = await response.json();
      if (response.status === 400) {
        if (data.error) {
          toast.error(data.error);
        }
        for (const field in data.errors) {
          data.errors[field].forEach((e: any) => toast.error(`${field}: ${e}`))
        };
      } else {
        toast.error(data.message);
      }
    }
  }

  const register = async (email: string, password: string) => {
    const response = await registerRequest(email, password);

    if (response.ok) {
      await getInfo();
    } else {
      const data = await response.json();
      if (response.status === 400) {
        if (data.error) {
          toast.error(data.error);
        }
        for (const field in data.errors) {
          data.errors[field].forEach((e: any) => toast(`${field}: ${e}`)
          )
        };
      }
      else {
        toast(data.message);
      }
    }
  }

  const getInfo = async () => {
    const infoResponse = await getUserInfo();
    if (infoResponse.ok) {
      setIsAuth(true);
      setUserDetails(await infoResponse.json());
      toast.success("Logged in");
    }
  }

  const logout = async () => {
    const response = await logoutRequest();
    if (response.ok) {
      setIsAuth(false);
      setUserDetails(null);
    }
  }

  useEffect(() => {
    if (!isAuth) {
      setIsLoadingData(true);
      getInfo();
      setIsLoadingData(false);
    }
  }, [isAuth, isLoadingData]);

  const value = {
    isAuth,
    userDetails,
    isLoading: isLoadingData,
    login,
    register,
    logout
  };

  return <UserContext.Provider value={value} {...props} />;
};

export const useUser = () => {
  const context = useContext(UserContext);
  if (context === undefined) {
    throw new Error(`useUser must be used within a MyUserContextProvider.`);
  }
  return context;
};

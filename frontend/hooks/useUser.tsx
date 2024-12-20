import { createContext, useContext, useEffect, useState } from "react";
import toast from "react-hot-toast";

import { UserDetails } from "@/types/types";
import getUserInfo from "@/services/auth/getUserInfo";
import logoutRequest from "@/services/auth/logout";

type UserContextType = {
  isAuth: boolean;
  userDetails: UserDetails | null;
  isLoading: boolean;
  authorize: (action: (form: FormData) => Promise<Response>, form: FormData) => Promise<void>;
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

  const getInfo = async () => {
    const infoResponse = await getUserInfo();
    if (infoResponse.ok) {
      setIsAuth(true);
      setUserDetails(await infoResponse.json());
      toast.success("Logged in");
    }
  }

  const authorize = async (action: (form: FormData) => Promise<Response>, form: FormData) => {
    setIsLoadingData(true);
    const response = await action(form);
    if (response.ok) {
      setIsAuth(true);
      toast.success("Authorized");
    } else {
      const exception = await response.json();
      toast.error(exception.detail);
    }
    setIsLoadingData(false);
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
    authorize,
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

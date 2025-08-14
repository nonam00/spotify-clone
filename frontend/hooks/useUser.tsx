import { createContext, useContext, useEffect, useState } from "react";
import toast from "react-hot-toast";

import {AuthSubmitType, UserDetails} from "@/types/types";
import getUserInfo from "@/services/auth/getUserInfo";
import logoutRequest from "@/services/auth/logout";
import login from "@/services/auth/login";
import register from "@/services/auth/register";

const actions = {
  "login": login,
  "register": register
}

type UserContextType = {
  isAuth: boolean;
  userDetails: UserDetails | null;
  isLoading: boolean;
  authorize: (submitType: AuthSubmitType, form: FormData) => Promise<boolean>;
  logout: () => Promise<void>;
}

export const UserContext = createContext<UserContextType | undefined>(
  undefined
);

export const MyUserContextProvider = ({
  children
}: {
  children: React.ReactNode
}) => {
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

  const authorize = async (
    submitType: AuthSubmitType,
    form: FormData
  ) => {
    setIsLoadingData(true);
    const action = actions[submitType];
    const response = await action(form);

    if (!response.ok) {
      const exception = await response.json();
      toast.error(exception.detail);
      setIsLoadingData(false);
      return false;
    }

    if (submitType == "login") {
      setIsAuth(true);
      toast.success("Authorized");
    } else if (submitType == "register") {
      toast.success("The confirmation code has been sent to your email. Activate your account and then login")
    }
    setIsLoadingData(false);
    return true;
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

  return <UserContext.Provider value={value}>{children}</UserContext.Provider>;
};

export const useUser = () => {
  const context = useContext(UserContext);
  if (context === undefined) {
    throw new Error(`useUser must be used within a MyUserContextProvider.`);
  }
  return context;
};

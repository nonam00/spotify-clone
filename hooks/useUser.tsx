import { createContext, useContext, useEffect, useState } from "react";
import Cookies from "js-cookie";

import AuthService from "@/api/services/AuthService";
import { UserDetails } from "@/types/types";

type UserContextType = {
  isAuth: boolean
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
    try {
      const { data } = await AuthService.login(email, password);
      setIsAuth(true);
      setUserDetails(data.user);
    } catch (e: any) {
      console.log(e.message);     
    }
  }

  const register = async (email: string, password: string) => {
    try {
      const response = await AuthService.registration(email, password);
      console.log(response);
    } catch (e: any) {
      console.log(e.response?.data?.message);     
    }
  }

  const logout = async () => {
    try {
      Cookies.remove("token");
      setIsAuth(false);
      setUserDetails(null); 
    } catch (e: any) {
      console.log(e?.message);     
    }
  }

  useEffect(() => {
    const accessToken = Cookies.get("token");
    if (!isAuth && accessToken !== null && accessToken !== undefined) {
      setIsLoadingData(true);
      Promise.allSettled([AuthService.getUserInfo()]).then(
        async (result) => {
          try {
            localStorage.setItem("token", accessToken);
            const response = await AuthService.getUserInfo();
            setUserDetails(response.data.user);
            setIsAuth(true);
        } catch(e: any) {
          console.log(e?.message);
        }
       }
      )
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

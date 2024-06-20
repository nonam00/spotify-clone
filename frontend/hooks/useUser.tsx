import { createContext, useContext, useEffect, useState } from "react";
import toast from "react-hot-toast";

import { UserDetails } from "@/types/types";
import $api from "@/api/http";

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
    await $api.post("/users/login/", { email, password })
      .then(async () => {
        await $api.get("/users/info/") 
          .then((response) => {
            setIsAuth(true);
            setUserDetails(response.data);
            toast.success("Logged In")    
          })
          .catch(error => console.log(error.message))
      })
      .catch((error) => {
        if (error.response.status === 400) {
          if(error.response.data.error) {
            toast.error(error.response.data.error);
          }
          for(const field in error.response.data.errors) {
            error.response.data.errors[field].forEach((e: any) => {
              toast.error(`${field}: ${e}`);
            });
          }
        }
        else if (error.response.status !== 200) {
          toast(error.response.data.message);
        }
      });
  }

  const register = async (email: string, password: string) => {
    await $api.post("/users/register/", { email, password })
      //.then(async () => {
        //await $api.get("users/info/") 
          //.then((response) => {
            //setIsAuth(true);
            //setUserDetails(response.data);
          //})
          //.catch(error => console.log(error.message))
      //})
      .catch((error) => {
        if (error.response.status === 400) {
          if(error.response.data.error) {
            toast.error(error.response.data.error);
          }
          for(const field in error.response.data.errors) {
            error.response.data.errors[field].forEach((e: any) => {
              toast(`${field}: ${e}`);
            });
          }
        }
        else if (error.response.status !== 200) {
          toast(error.response.data.message);
        }
      });
  }

  const logout = async () => {
    try {
      await $api.post("users/logout/");
      setIsAuth(false);
      setUserDetails(null);
    } catch (e: any) {
      console.log(e?.message);     
    }
  }

  useEffect(() => {
    if (!isAuth) {
      setIsLoadingData(true);
      Promise.allSettled([$api.get("users/info")]).then(
        async (results) => {
          try {
            if (results[0].status === "fulfilled") {
              setUserDetails(results[0].value.data?.user);
              setIsAuth(true);
            } else {
              throw new Error("Error on loading data")
            }
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

"use client";

import { toast } from "react-hot-toast";
import { useEffect, useState } from "react";
import { AxiosError } from "axios";
import { AiFillHeart, AiOutlineHeart } from "react-icons/ai";

import $api, { API_URL } from "@/api/http";

import useAuthModal from "@/hooks/useAuthModal";
import { useUser } from "@/hooks/useUser";

interface LikeButtonProps {
  songId: string
}

const LikeButton: React.FC<LikeButtonProps> = ({
  songId
}) => {
  const authModal = useAuthModal();
  const { isAuth } = useUser();
  const [isLiked, setIsLiked] = useState<boolean>(false);
  
  useEffect(() => {
    if(!isAuth) {
      return;
    }

    const fetchData = async () => {
      await $api.get(`users/songs/${songId}`)
        .then((response) => {
          if (response.status >= 200 && response.status < 400 && response.data) {
            setIsLiked(true);
          }
        })
        .catch((error: AxiosError) => {
          console.log(error.response?.data);
        });
    }
    fetchData();
  }, [songId, isAuth, isLiked]);

  const handleLike = async () => {
    if (!isAuth) {
      return authModal.onOpen()
    }

    if (isLiked) {
      await $api.delete(`/users/songs/${songId}`)
        .then(() => {
          setIsLiked(false);
          toast.success("Like deleted");
        })
        .catch((error: AxiosError) => {
          toast.error("An error occurred while deleting the song from your favorites");
          console.log(error.response?.data);
        });

    } else {
        await fetch(`${API_URL}/1/users/songs/${songId}`, {
          method: 'POST',
          credentials: 'include'
        })
          .then(() => {
            setIsLiked(true);
            toast.success('Liked');
          })
          .catch((error: AxiosError) => {
            toast.error("An error occurred while adding the song to the favorites");
            console.log(error.message);
          });
    }
  }
  return (
    <button
      onClick={handleLike}
      className="hover:opacity-75 transition"
    >
      { isLiked
        ? <AiFillHeart color="22c55e" size={25}/>
        : <AiOutlineHeart color="white" size={25}/>
      }
    </button>
  );
}
 
export default LikeButton;

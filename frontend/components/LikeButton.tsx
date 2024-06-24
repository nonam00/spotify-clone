"use client";

import { useRouter } from "next/navigation";
import { toast } from "react-hot-toast";
import { useEffect, useState } from "react";
import { AxiosError } from "axios";
import { AiFillHeart, AiOutlineHeart } from "react-icons/ai";

import $api from "@/api/http";

import useAuthModal from "@/hooks/useAuthModal";
import { useUser } from "@/hooks/useUser";

import { Song } from "@/types/types";

interface LikeButtonProps {
  songId: string
}

const LikeButton: React.FC<LikeButtonProps> = ({
  songId
}) => {
  const router = useRouter();
  
  const authModal = useAuthModal();
  const user = useUser();

  const [isLiked, setIsLiked] = useState<boolean>(false);
  
  useEffect(() => {
    if(!user.isAuth) {
      return;
    }

    const fetchData = async () => {
      await $api.get<Song>(`/liked/get/${songId}`)
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
  }, [songId, user.isAuth]);

  const Icon = isLiked? AiFillHeart : AiOutlineHeart;

  const handleLike = async () => {
    if (!user.isAuth) {
      return authModal.onOpen()
    }

    if (isLiked) {
      await $api.delete(`/liked/delete/${songId}`)
        .then(() => {
          setIsLiked(false);
          toast("Like deleted");
        })
        .catch((error: AxiosError) => {
          toast("An error occurred while deleting the song from your favorites");
          console.log(error.response?.data);
        });

    } else {
        await fetch(`https://localhost:7025/1/liked/like/${songId}`, {
          method: 'POST',
          credentials: 'include'
        })
          .then(() => {
            setIsLiked(true);
            toast.success('Liked');
          })
          .catch((error: AxiosError) => {
            toast("An error occurred while adding the song to the favorites");
            console.log(error.message);
          });
    }
    router.refresh();
  }
  return (
    <button
      onClick={handleLike}
      className="
        hover:opacity-75
        transition
      "
    >
      <Icon color={isLiked ? '22c55e' : 'white'} size={25}/>
    </button>
  );
}
 
export default LikeButton;

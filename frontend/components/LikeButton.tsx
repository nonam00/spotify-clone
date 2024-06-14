"use client";

import { useRouter } from "next/navigation";
import { toast } from "react-hot-toast";
import { useEffect, useState } from "react";
import { AiFillHeart, AiOutlineHeart } from "react-icons/ai";
import Cookies from "js-cookie";

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
      try {
        const { data } = await $api.get<Song>(`/liked/get/${songId}`);
        if (data) {
          setIsLiked(true);
        }
      } catch(error) {
        console.log(error);
      }
    };

    fetchData();
  }, [songId, user.isAuth]);

  const Icon = isLiked? AiFillHeart : AiOutlineHeart;

  const handleLike = async () => {
    if (!user.isAuth) {
      return authModal.onOpen()
    }

    if (isLiked) {
      try {
        await $api.delete(`/liked/delete/${songId}`);
        setIsLiked(false);
        toast("Like deleted");
      } catch(error: any) {
        toast(error?.message);
      }
    } else {
        await fetch(`https:localhost:7025/1/liked/like/${songId}`, {
          method: 'POST'
        })
          .then(() => {
            setIsLiked(true);
            toast.success('Liked');
          })
          .catch((error: Error) => toast(error.message));
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
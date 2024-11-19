"use client";

import { toast } from "react-hot-toast";
import { useEffect, useState } from "react";
import { AiFillHeart, AiOutlineHeart } from "react-icons/ai";

import useAuthModal from "@/hooks/useAuthModal";
import { useUser } from "@/hooks/useUser";
import checkLikedSong from "@/services/liked/checkLikedSong";
import addLikedSong from "@/services/liked/addLikedSong";
import deleteLikedSong from "@/services/liked/deleteLikedSong";

interface LikeButtonProps {
  songId: string;
  defaultValue?: boolean;
}

const LikeButton: React.FC<LikeButtonProps> = ({
  songId, 
  defaultValue
}) => {
  const openAuthModal = useAuthModal(s => s.onOpen);
  const { isAuth } = useUser();
  const [isLiked, setIsLiked] = useState<boolean>(defaultValue ?? false);

  useEffect(() => {
    if (!isAuth) {
      return;
    }
    if(!defaultValue) {
      const fetchData = async () => {
        const response = await checkLikedSong(songId);
        if (response.ok) {
          const data = await response.json();
          if (data) {
            setIsLiked(true);
          }
        }
      }
      fetchData();
    }
  }, [songId, isAuth, isLiked, defaultValue]);

  const handleLike = async () => {
    if (!isAuth) {
      return openAuthModal();
    }

    if (isLiked) {
      const response = await deleteLikedSong(songId);
      if (response.ok) {
        setIsLiked(false)
        toast.success("Like deleted")
      } else {
        toast.error("An error occurred while deleting the song from your favorites");
      }
    } else {
      const response = await addLikedSong(songId); 
      if (response.ok) {
        setIsLiked(true);
        toast.success('Liked');
      } else {
        toast.error("An error occurred while adding the song to the favorites");
      }
    }
  }
  return (
    <button
      onClick={handleLike}
      className="hover:opacity-75 transition"
    >
      {isLiked
        ? <AiFillHeart color="22c55e" size={25} />
        : <AiOutlineHeart color="white" size={25} />
      }
    </button>
  );
}

export default LikeButton;

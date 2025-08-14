"use client";

import { toast } from "react-hot-toast";
import {useLayoutEffect, useState, useTransition} from "react";
import { AiFillHeart, AiOutlineHeart } from "react-icons/ai";

import useAuthModal from "@/hooks/useAuthModal";
import { useUser } from "@/hooks/useUser";
import checkLikedSong from "@/services/liked/checkLikedSong";
import addLikedSong from "@/services/liked/addLikedSong";
import deleteLikedSong from "@/services/liked/deleteLikedSong";

const LikeButton = ({
  songId, 
  defaultValue
}: {
  songId: string;
  defaultValue?: boolean;
}) => {
  const openAuthModal = useAuthModal(s => s.onOpen);
  const { isAuth } = useUser();
  const [isLiked, setIsLiked] = useState<boolean>(defaultValue ?? false);
  const [isPending, startTransition] = useTransition();

  useLayoutEffect(() => {
    if (!isAuth || defaultValue !== undefined) {
      return;
    }

    const abortController = new AbortController();

    const loadLike = async () => {
      const response = await checkLikedSong(songId, abortController);
      if (response.ok) {
        const data = await response.json();
        if (data.check) {
          setIsLiked(true);
        }
      }
    };

    loadLike();

    return () => {
      abortController.abort();
    }
  }, [songId, isAuth, isLiked, defaultValue]);

  const handleLike = async () => {
    if (!isAuth) {
      return openAuthModal();
    }

    startTransition(async () => {
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
    });
  }

  return (
    <button
      onClick={handleLike}
      disabled={isPending}
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

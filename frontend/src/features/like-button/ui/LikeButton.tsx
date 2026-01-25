"use client";

import { useLayoutEffect, useState, useTransition } from "react";
import { toast } from "react-hot-toast";
import { AiFillHeart, AiOutlineHeart } from "react-icons/ai";
import {
  addLikedSong,
  checkLikedSong,
  deleteLikedSong,
} from "../api";

const LikeButton = ({
  songId,
  defaultValue,
}: {
  songId: string;
  defaultValue?: boolean;
}) => {
  const [isLiked, setIsLiked] = useState<boolean>(defaultValue ?? false);
  const [isPending, startTransition] = useTransition();

  useLayoutEffect(() => {
    if (defaultValue !== undefined) {
      return;
    }
    const abortController = new AbortController();

    async function loadLike(){
      const success = await checkLikedSong(songId, abortController);
      setIsLiked(success);
    }
    loadLike();

    return () => {
      abortController.abort();
    };
  }, [defaultValue, songId]);

  const handleLike = async () => {
    startTransition(async () => {
      if (isLiked) {
        const success = await deleteLikedSong(songId);

        if (success) {
          setIsLiked(false);
          toast.success("Like deleted");
        } else {
          toast.error("An error occurred while deleting the song from your favorites");
        }
      } else {
        const success = await addLikedSong(songId);

        if (success) {
          setIsLiked(true);
          toast.success("Liked");
        } else {
          toast.error("An error occurred while adding the song to the favorites");
        }
      }
    });
  };

  return (
    <button
      onClick={handleLike}
      disabled={isPending}
      className="p-2 hover:opacity-75 transition cursor-pointer"
    >
      {isLiked ? (
        <AiFillHeart color="#22c55e" size={25} />
      ) : (
        <AiOutlineHeart color="white" size={25} />
      )}
    </button>
  );
};

export default LikeButton;
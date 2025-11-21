"use client";

import { toast } from "react-hot-toast";
import { useLayoutEffect, useState, useTransition } from "react";
import { AiFillHeart, AiOutlineHeart } from "react-icons/ai";

import { useAuthModalStore, useAuthStore } from "@/features/auth";
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
  const openAuthModal = useAuthModalStore((s) => s.onOpen);
  const { isAuthenticated } = useAuthStore();
  const [isLiked, setIsLiked] = useState<boolean>(defaultValue ?? false);
  const [isPending, startTransition] = useTransition();

  useLayoutEffect(() => {
    if (!isAuthenticated || defaultValue !== undefined) {
      return;
    }

    const abortController = new AbortController();

    const loadLike = async () => {
      const success = await checkLikedSong(songId, abortController);
      setIsLiked(success);
    };

    loadLike();

    return () => {
      abortController.abort();
    };
  }, [songId, isAuthenticated, defaultValue]);

  const handleLike = async () => {
    if (!isAuthenticated) {
      return openAuthModal();
    }

    startTransition(async () => {
      if (isLiked) {
        const success = await deleteLikedSong(songId);
        if (success) {
          setIsLiked(false);
          toast.success("Like deleted");
        } else {
          toast.error(
            "An error occurred while deleting the song from your favorites"
          );
        }
      } else {
        const success = await addLikedSong(songId);
        if (success) {
          setIsLiked(true);
          toast.success("Liked");
        } else {
          toast.error(
            "An error occurred while adding the song to the favorites"
          );
        }
      }
    });
  };

  return (
    <button
      onClick={handleLike}
      disabled={isPending}
      className="hover:opacity-75 transition cursor-pointer"
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
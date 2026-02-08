"use client";

import { useCallback, useEffect, useRef, useState } from "react";
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
  const [isLoading, setIsLoading] = useState(false);
  const abortControllerRef = useRef<AbortController | null>(null);

  const loadLikeStatus = useCallback(async (signal: AbortSignal) => {
    try {
      const success = await checkLikedSong(songId, signal);
      if (!signal.aborted) {
        setIsLiked(success);
      }
    } catch (error) {
      if (error instanceof DOMException && error.name === "AbortError") {
        return;
      }
      console.error("Error checking liked song: ", error);
    }
  }, [songId]);

  useEffect(() => {
    // Dont check if has default value
    if (defaultValue !== undefined) {
      return;
    }

    abortControllerRef.current = new AbortController();
    const signal = abortControllerRef.current.signal;

    loadLikeStatus(signal);

    return () => {
      abortControllerRef.current?.abort();
      abortControllerRef.current = null;
    };
  }, [defaultValue, loadLikeStatus, songId]);

  const handleLike = async () => {
    // Preventing race condition
    if (isLoading) return;

    const previousState = isLiked;

    // Optimistic update
    setIsLoading(!isLiked);
    setIsLoading(true);

    try {
      if (previousState) {
        const success = await deleteLikedSong(songId);
        if (!success) {
          toast.error("An error occurred while deleting the song from your favorites");
          setIsLiked(previousState); // Rollback
        } else {
          toast.success("Like deleted");
        }
      } else {
        const success = await addLikedSong(songId);
        if (!success) {
          toast.error("An error occurred while adding the song to the favorites");
          setIsLiked(previousState); // Rollback
        } else {
          toast.success("Liked");
        }
      }
    } catch (error) {
      console.error("Error updating like status: ", error);
      setIsLiked(previousState);
      toast.error("An error occurred. Please try again.");
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <button
      onClick={handleLike}
      disabled={isLoading}
      className="p-2 hover:opacity-75 transition cursor-pointer"
      aria-label={isLiked ? "Unliked song" : "Like song"}
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
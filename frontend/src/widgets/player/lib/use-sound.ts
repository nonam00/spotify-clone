"use client";

import { Song}  from "@/entities/song";
import { RefObject, useEffect, useRef, useState } from "react";

type UseSoundReturnType = {
  audioRef: RefObject<HTMLAudioElement | null>;
  isPlaying: boolean;
  duration: number;
  currentTime: number;
  isStalled: boolean;
  isSeeking: boolean;
};

// Manages audio component and Media API
export function useSound(
  song: Song | undefined,
  songUrl: string | undefined,
  volume: number,
  setNextSong: () => void,
  setPreviousSong: () => void
): UseSoundReturnType {
  const audioRef = useRef<HTMLAudioElement>(null);
  const [isPlaying, setIsPlaying] = useState(false);
  const [currentTime, setCurrentTime] = useState(0);
  const [duration, setDuration] = useState(0);
  const [isStalled, setIsStalled] = useState(false);
  const [isSeeking, setIsSeeking] = useState(false);

  const isMountedRef = useRef(true);

  useEffect(() => {
    isMountedRef.current = true;
    return () => {
      isMountedRef.current = false;
    };
  }, []);

  // Reset state when song changes
  useEffect(() => {
    setIsPlaying(false);
    setCurrentTime(0);
    setDuration(0);
    setIsStalled(false);
    setIsSeeking(false);
  }, [song?.id]);

  // Handle song URL changes
  useEffect(() => {
    if (!audioRef.current || !songUrl || !song) {
      // Pause and reset if no song URL or song
      if (audioRef.current) {
        audioRef.current.pause();
      }
      setIsPlaying(false);
      setIsStalled(false);
      setIsSeeking(false);
      return;
    }

    const audio = audioRef.current;
    let shouldAutoPlay = true;

    setIsPlaying(false);
    setCurrentTime(0);
    setDuration(0);

    let isCurrentAudioValid = true;

    const updateTime = () => {
      if (isMountedRef.current && isCurrentAudioValid) {
        setCurrentTime(audio.currentTime);
      }
    };

    const updateDuration = () => {
      if (isMountedRef.current && isCurrentAudioValid) {
        setDuration(audio.duration);
      }
    };

    const handleEnded = () => {
      if (isMountedRef.current && isCurrentAudioValid) {
        setIsStalled(false);
        setNextSong();
      }
    };

    const handlePlayEvent = () => {
      if (isMountedRef.current && isCurrentAudioValid) {
        setIsPlaying(true);
      }
    };

    const handlePause = () => {
      if (isMountedRef.current && isCurrentAudioValid) {
        setIsPlaying(false);
        setIsStalled(false);
      }
    };

    const handleLoadStart = () => {
      if (isMountedRef.current && isCurrentAudioValid) {
        setIsPlaying(false);
        setIsStalled(true);
      }
    };

    const handleLoadEnd = () => {
      if (isMountedRef.current && isCurrentAudioValid) {
        setIsStalled(false);
      }
    }

    const handleSeeking = () => {
      if (isMountedRef.current && isCurrentAudioValid) {
        setIsSeeking(true);
      }
    }

    const handleSeekingEnd = () => {
      if (isMountedRef.current && isCurrentAudioValid) {
        setIsSeeking(false);
      }
    }

    const handleError = (e: ErrorEvent) => {
      console.error("Audio error:", e.error);
    };

    const handleCanPlay = () => {
      if (shouldAutoPlay && isMountedRef.current && isCurrentAudioValid) {
        setIsStalled(false);
        // Autoplay if mount is relevant
        audio.play().catch((error) => {
          if (isMountedRef.current && isCurrentAudioValid) {
            console.error("Autoplay error:", error);
            setIsPlaying(false);
          }
        });
        shouldAutoPlay = false;
      }
    };

    const handleWaiting = () => {
      if (isMountedRef.current && isCurrentAudioValid) {
        setIsStalled(true);
      }
    };

    const handlePlaying = () => {
      if (isMountedRef.current && isCurrentAudioValid) {
        setIsStalled(false);
      }
    };

    const togglePlay = () => {
      if (isMountedRef.current && isCurrentAudioValid) {
        if (!audio.paused) {
          audio.pause();
        } else {
          audio.play().catch((error) => {
            if (isMountedRef.current && isCurrentAudioValid) {
              console.error("Media API play error:", error);
              setIsPlaying(false);
            }
          });
        }
      }
    }

    const handleKeyDown = (event: KeyboardEvent) => {
      if (event.key === ' ' || event.key === 'Space') {
        event.preventDefault();
        togglePlay();
      }
    }

    audio.src = songUrl;
    audio.volume = volume;

    audio.addEventListener("timeupdate", updateTime);
    audio.addEventListener("loadedmetadata", updateDuration);
    audio.addEventListener("ended", handleEnded);
    audio.addEventListener("play", handlePlayEvent);
    audio.addEventListener("pause", handlePause);

    audio.addEventListener("loadstart", handleLoadStart);
    audio.addEventListener("loadeddata", handleLoadEnd);
    audio.addEventListener("canplay", handleCanPlay);
    audio.addEventListener("waiting", handleWaiting);
    audio.addEventListener("playing", handlePlaying);

    audio.addEventListener("seeking", handleSeeking);
    audio.addEventListener("seeked", handleSeekingEnd);

    audio.addEventListener('error', handleError);

    document.addEventListener('keydown', handleKeyDown);

    // Configuration of media player widget in user device
    if ("mediaSession" in navigator) {
      navigator.mediaSession.metadata = new MediaMetadata({
        title: song.title,
        artist: song.author,
        artwork: [
          {
            src: song.imagePath,
            sizes: "512x512",
            type: "image/*",
          },
        ],
      });

      navigator.mediaSession.setActionHandler("play", () => {
        if (isCurrentAudioValid) {
          audio.play().catch((error) => {
            if (isMountedRef.current && isCurrentAudioValid) {
              console.error("Media API play error:", error);
              setIsPlaying(false);
            }
          });
        }
      });
      navigator.mediaSession.setActionHandler("pause", () => {
        if (isCurrentAudioValid) {
          audio.pause();
        }
      });
      navigator.mediaSession.setActionHandler("previoustrack", setPreviousSong);
      navigator.mediaSession.setActionHandler("nexttrack", setNextSong);
      navigator.mediaSession.setActionHandler("seekto", (details) => {
        if (details.seekTime !== undefined && isCurrentAudioValid) {
          audio.currentTime = details.seekTime;
        }
      });
    }

    return () => {
      isCurrentAudioValid = false;

      if (audio) {
        audio.pause();

        document.removeEventListener('keydown', handleKeyDown);

        audio.removeEventListener("timeupdate", updateTime);
        audio.removeEventListener("loadedmetadata", updateDuration);
        audio.removeEventListener("ended", handleEnded);
        audio.removeEventListener("play", handlePlayEvent);
        audio.removeEventListener("pause", handlePause);

        audio.removeEventListener("loadstart", handleLoadStart);
        audio.removeEventListener("loadeddata", handleLoadEnd);
        audio.removeEventListener("canplay", handleCanPlay);
        audio.removeEventListener("waiting", handleWaiting);
        audio.removeEventListener("playing", handlePlaying);

        audio.removeEventListener("seeking", handleSeeking);
        audio.removeEventListener("seeked", handleSeekingEnd);

        audio.removeEventListener('error', handleError);
      }

      if ("mediaSession" in navigator) {
        navigator.mediaSession.metadata = null;

        navigator.mediaSession.setActionHandler("play", null);
        navigator.mediaSession.setActionHandler("pause", null);
        navigator.mediaSession.setActionHandler("nexttrack", null);
        navigator.mediaSession.setActionHandler("previoustrack", null);
        navigator.mediaSession.setActionHandler("seekto", null);
      }
    };
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [songUrl, song, setNextSong, setPreviousSong]);

  // Handle volume changes
  useEffect(() => {
    if (audioRef.current) {
      audioRef.current.volume = volume;
    }
  }, [volume]);

  return { audioRef, isPlaying, duration, currentTime, isStalled, isSeeking };
}
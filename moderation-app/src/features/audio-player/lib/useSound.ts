import { type RefObject, useEffect, useRef, useState } from "react";

type UseSoundReturnType = {
  audioRef: RefObject<HTMLAudioElement | null>;
  isPlaying: boolean;
  isStalled: boolean;
  duration: number;
  currentTime: number;
  togglePlay: () => void;
  setCurrentTime: (time: number) => void;
  isSeeking: boolean;
};

export function useSound(
  songUrl: string,
  volume: number = 1
): UseSoundReturnType {
  const audioRef = useRef<HTMLAudioElement>(null);
  const [isPlaying, setIsPlaying] = useState(false);
  const [currentTime, setCurrentTime] = useState(0);
  const [duration, setDuration] = useState(0);
  const [isStalled, setIsStalled] = useState(false);
  const [isSeeking, setIsSeeking] = useState(false);

  // Handle song URL changes
  useEffect(() => {
    if (!audioRef.current) return;

    const audio = audioRef.current;
    audio.src = songUrl;
    audio.volume = volume;

    const updateTime = () => setCurrentTime(audio.currentTime);
    const updateDuration = () => setDuration(audio.duration);
    const handlePlayEvent = () => setIsPlaying(true);
    const handlePause = () => setIsPlaying(false);
    const handleLoadEnd = () => setIsStalled(false);
    const handleWaiting = () => setIsStalled(true);
    const handlePlaying = () => setIsStalled(false);
    const handleSeeking = () => setIsSeeking(true);
    const handleSeekingEnd = () => setIsSeeking(false);

    const handleLoadStart = () => {
      setIsPlaying(false);
      setIsStalled(true);
    }

    const handleError = (e: ErrorEvent) => console.error("Audio error:", e.error);

    audio.addEventListener('play', handlePlayEvent);
    audio.addEventListener('pause', handlePause);
    audio.addEventListener('timeupdate', updateTime);
    audio.addEventListener('loadedmetadata', updateDuration);

    audio.addEventListener("loadstart", handleLoadStart);
    audio.addEventListener("loadeddata", handleLoadEnd);
    audio.addEventListener("waiting", handleWaiting);
    audio.addEventListener("playing", handlePlaying);

    audio.addEventListener("seeking", handleSeeking);
    audio.addEventListener("seeked", handleSeekingEnd);

    audio.addEventListener("error", handleError);

    return () => {
      audio.pause();
      audio.removeEventListener('play', handlePlayEvent);
      audio.removeEventListener('pause', handlePause);
      audio.removeEventListener('timeupdate', updateTime);
      audio.removeEventListener('loadedmetadata', updateDuration);

      audio.removeEventListener("loadstart", handleLoadStart);
      audio.removeEventListener("loadeddata", handleLoadEnd);
      audio.removeEventListener("waiting", handleWaiting);
      audio.removeEventListener("playing", handlePlaying);

      audio.removeEventListener("seeking", handleSeeking);
      audio.removeEventListener("seeked", handleSeekingEnd);

      audio.removeEventListener("error", handleError);
    };
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [songUrl]);

  // Handle volume changes
  useEffect(() => {
    if (audioRef.current) {
      audioRef.current.volume = volume;
    }
  }, [volume]);

  const togglePlay = () => {
    if (!audioRef.current) return;

    if (isPlaying) {
      audioRef.current.pause();
    } else {
      audioRef.current.play().catch(error => {
        console.error("Play error:", error);
      });
    }
  };

  const setCurrentTimeHandler = (time: number) => {
    if (audioRef.current) {
      audioRef.current.currentTime = time;
    }
  };

  return { 
    audioRef, 
    isPlaying,
    isStalled,
    duration,
    currentTime, 
    togglePlay,
    setCurrentTime: setCurrentTimeHandler,
    isSeeking
  };
}
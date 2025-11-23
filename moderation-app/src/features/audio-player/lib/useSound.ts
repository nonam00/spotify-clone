import { type RefObject, useEffect, useRef, useState } from "react";

type UseSoundReturnType = {
  audioRef: RefObject<HTMLAudioElement | null>;
  isPlaying: boolean;
  duration: number;
  currentTime: number;
  togglePlay: () => void;
  setCurrentTime: (time: number) => void;
};

export function useSound(songUrl: string, volume: number = 1): UseSoundReturnType {
  const audioRef = useRef<HTMLAudioElement>(null);
  const [isPlaying, setIsPlaying] = useState(false);
  const [currentTime, setCurrentTime] = useState(0);
  const [duration, setDuration] = useState(0);

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

    audio.addEventListener('play', handlePlayEvent);
    audio.addEventListener('pause', handlePause);
    audio.addEventListener('timeupdate', updateTime);
    audio.addEventListener('loadedmetadata', updateDuration);

    return () => {
      audio.pause();
      audio.removeEventListener('play', handlePlayEvent);
      audio.removeEventListener('pause', handlePause);
      audio.removeEventListener('timeupdate', updateTime);
      audio.removeEventListener('loadedmetadata', updateDuration);
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
    duration, 
    currentTime, 
    togglePlay,
    setCurrentTime: setCurrentTimeHandler
  };
}
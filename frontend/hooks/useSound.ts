import {RefObject, useEffect, useRef, useState} from "react";

type useSoundReturnType = {
  audioRef: RefObject<HTMLAudioElement | null>,
  isPlaying: boolean,
  duration: number,
  currentTime: number
}

function useSound(songUrl: string, volume: number, setNextSong: () => void): useSoundReturnType {
  const audioRef = useRef<HTMLAudioElement>(null);
  const [isPlaying, setIsPlaying] = useState(false);
  const [currentTime, setCurrentTime] = useState(0);
  const [duration, setDuration] = useState(0);

  // Handle song URL changes
  useEffect(() => {
    if (!audioRef.current) return;

    const audio = audioRef.current;
    audio.src = songUrl;

    const updateTime = () => setCurrentTime(audio.currentTime);
    const updateDuration = () => setDuration(audio.duration);
    const handleEnded = () => setNextSong();
    const handlePlayEvent = () => setIsPlaying(true);
    const handlePause = () => setIsPlaying(false);

    audio.addEventListener('play', handlePlayEvent);
    audio.addEventListener('pause', handlePause);
    audio.addEventListener('timeupdate', updateTime);
    audio.addEventListener('loadedmetadata', updateDuration);
    audio.addEventListener('ended', handleEnded);

    // Attempt autoplay and handle errors
    audio.play().catch(error => {
      console.log("Autoplay error:", error);
      setIsPlaying(false);
    });

    return () => {
      audio.pause();
      audio.removeEventListener('play', handlePlayEvent);
      audio.removeEventListener('pause', handlePause);
      audio.removeEventListener('timeupdate', updateTime);
      audio.removeEventListener('loadedmetadata', updateDuration);
      audio.removeEventListener('ended', handleEnded);
    };
  }, [songUrl]);

  // Handle volume changes
  useEffect(() => {
    if (audioRef.current) {
      audioRef.current.volume = volume;
    }
  }, [volume]);

  return { audioRef, isPlaying, duration, currentTime };
}

export default useSound;
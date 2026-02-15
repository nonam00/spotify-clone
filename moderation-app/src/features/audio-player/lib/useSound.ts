import { type RefObject, useEffect, useRef, useState } from "react";

type UseSoundReturnType = {
  audioRef: RefObject<HTMLAudioElement | null>;
  isPlaying: boolean;
  isStalled: boolean;
  duration: number;
  currentTime: number;
  isSeeking: boolean;
};

export function useSound(songUrl: string,): UseSoundReturnType {
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

    const handleTimeUpdate = () => setCurrentTime(audio.currentTime);
    const handleLoadedMetadata = () => setDuration(audio.duration);
    const handlePlay = () => setIsPlaying(true);
    const handlePause = () => setIsPlaying(false);
    const handleLoadEnd = () => setIsStalled(false);
    const handleWaiting = () => setIsStalled(true);
    const handlePlaying = () => setIsStalled(false);
    const handleSeeking = () => setIsSeeking(true);
    const handleSeeked = () => setIsSeeking(false);
    const handleLoadStart = () => {
      setIsPlaying(false);
      setIsStalled(true);
    };
    const handleError = (e: ErrorEvent) => console.error("Audio error:", e.error);

    // Attach event listeners
    const events: [string, EventListenerOrEventListenerObject][] = [
      ["timeupdate", handleTimeUpdate],
      ["loadedmetadata", handleLoadedMetadata],
      ["play", handlePlay],
      ["pause", handlePause],
      ["loadstart", handleLoadStart],
      ["loadeddata", handleLoadEnd],
      ["waiting", handleWaiting],
      ["playing", handlePlaying],
      ["seeking", handleSeeking],
      ["seeked", handleSeeked],
      ["error", handleError as EventListenerOrEventListenerObject],
    ];

    events.forEach(([event, handler]) => audio.addEventListener(event, handler));

    return () => {
      audio.pause();
      events.forEach(([event, handler]) => audio.removeEventListener(event, handler));
    };
  }, [songUrl]);

  return { 
    audioRef, 
    isPlaying,
    isStalled,
    duration,
    currentTime, 
    isSeeking,
  };
}
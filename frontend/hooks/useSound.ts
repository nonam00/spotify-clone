import {RefObject, useEffect, useRef, useState} from "react";
import {Song} from "@/types/types";

type useSoundReturnType = {
  audioRef: RefObject<HTMLAudioElement | null>,
  isPlaying: boolean,
  duration: number,
  currentTime: number
}

function useSound(song: Song, songUrl: string, volume: number, setNextSong: () => void, setPreviousSong: () => void): useSoundReturnType {
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
      console.error("Autoplay error:", error);
      setIsPlaying(false);
    });

    // Configuration of media player widget in user device
    if ("mediaSession" in navigator) {
      navigator.mediaSession.metadata = new MediaMetadata({
        title: song.title,
        artist: song.author,
        artwork: [{
          src: song.imagePath,
          sizes: "512x512",
          type: "image/*",
        }]
      });

      navigator.mediaSession.setActionHandler('play', () => audio.play());
      navigator.mediaSession.setActionHandler('pause', () => audio.pause());
      navigator.mediaSession.setActionHandler('previoustrack', setPreviousSong);
      navigator.mediaSession.setActionHandler('nexttrack', setNextSong);
      navigator.mediaSession.setActionHandler('seekto', (details) => {
        if (details.seekTime !== undefined) {
          audio.currentTime = details.seekTime;
        }
      })
    }


    return () => {
      audio.pause();

      audio.removeEventListener('play', handlePlayEvent);
      audio.removeEventListener('pause', handlePause);
      audio.removeEventListener('timeupdate', updateTime);
      audio.removeEventListener('loadedmetadata', updateDuration);
      audio.removeEventListener('ended', handleEnded);

      navigator.mediaSession.metadata = null;
      navigator.mediaSession.setActionHandler('play', null);
      navigator.mediaSession.setActionHandler('pause', null);
      navigator.mediaSession.setActionHandler('nexttrack', null);
      navigator.mediaSession.setActionHandler('previoustrack', null);
      navigator.mediaSession.setActionHandler('seekto', null);
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
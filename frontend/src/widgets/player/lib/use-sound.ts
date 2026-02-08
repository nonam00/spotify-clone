"use client";

import { Song } from "@/entities/song";
import { RefObject, useEffect, useRef, useState } from "react";
import { useShallow } from "zustand/shallow";
import { isEditableElement } from "./helpers";
import { usePlayerStore } from "@/widgets/player";
import {CLIENT_FILES_URL} from "@/shared/config/api";

type UseSoundReturnType = {
  audioRef: RefObject<HTMLAudioElement | null>;
  isPlaying: boolean;
  duration: number;
  currentTime: number;
  isStalled: boolean;
  isSeeking: boolean;
};

type UseSoundProps = {
  song: Song | undefined;
  songUrl: string | undefined;
};

export function useSound({ song, songUrl }: UseSoundProps): UseSoundReturnType {
  const { isRehydrated, volume, setNextSong, setPreviousSong, setRehydrated } = usePlayerStore(
    useShallow((s) => ({
      isRehydrated: s.isRehydrated,
      volume: s.volume,
      setNextSong: s.setNextId,
      setPreviousSong: s.setPreviousId,
      setRehydrated: s.setRehydrated,
    }))
  );

  const audioRef = useRef<HTMLAudioElement>(null);
  const [isPlaying, setIsPlaying] = useState(false);
  const [currentTime, setCurrentTime] = useState(0);
  const [duration, setDuration] = useState(0);
  const [isStalled, setIsStalled] = useState(false);
  const [isSeeking, setIsSeeking] = useState(false);

  // Reset state when song changes
  useEffect(() => {
    setIsPlaying(false);
    setCurrentTime(0);
    setDuration(0);
    setIsStalled(false);
    setIsSeeking(false);
  }, [song?.id]);

  // Handle volume changes separately from the main effect, reason described bellow
  useEffect(() => {
    if (audioRef.current) {
      audioRef.current.volume = volume;
    }
  }, [volume]);

  // Main effect for audio setup
  useEffect(() => {
    if (!audioRef.current || !songUrl || !song) {
      audioRef.current?.pause();
      setIsPlaying(false);
      setIsStalled(false);
      return;
    }

    const audio = audioRef.current;
    let isActive = true;

    // Setup audio element
    audio.src = songUrl;
    audio.volume = volume;

    // Event handlers
    const handleTimeUpdate = () => isActive && setCurrentTime(audio.currentTime);
    const handleLoadedMetadata = () => isActive && setDuration(audio.duration);
    const handleEnded = () => isActive && (setIsStalled(false), setNextSong());
    const handlePlay = () => isActive && setIsPlaying(true);
    const handlePause = () => isActive && (setIsPlaying(false), setIsStalled(false));
    const handleLoadStart = () => isActive && (setIsPlaying(false), setIsStalled(true));
    const handleCanPlay = () => isActive && setIsStalled(false);
    const handleWaiting = () => isActive && setIsStalled(true);
    const handleSeeking = () => isActive && setIsSeeking(true);
    const handleSeeked = () => isActive && setIsSeeking(false);
    const handleError = (e: ErrorEvent) => console.error("Audio error:", e.error);

    // Try autoplay when audio can play (song file was loaded)
    const handleCanPlayThrough = () => {
      // If player storage was rehydrated autoplay on player render is not allowed
      if (isActive && audio.paused && !isRehydrated) {
        audio.play()
          .catch((error) => {
            // Autoplay failed (e.g., browser policy, user hasn't interacted)
            console.log("Autoplay prevented:", error);
            setIsPlaying(false);
          });
      }
    };

    // Spacebar handler
    const handleKeyDown = (event: KeyboardEvent) => {
      if (!audio || isEditableElement(event.target as HTMLElement) || event.code !== "Space") {
        return;
      }
      event.preventDefault();

      // Toggle play/pause based on current audio state
      if (audio.paused) {
        audio.play().catch(console.error);
      } else {
        audio.pause();
      }
    };

    // Media Session setup
    const setupMediaSession = () => {
      if (!("mediaSession" in navigator) || !song) return;

      const sizes = ['96x96', '128x128', '192x192', '256x256', '384x384', '512x512'];

      navigator.mediaSession.metadata = new MediaMetadata({
        title: song.title,
        artist: song.author,
        artwork: sizes.map((size) => ({
          src: `${CLIENT_FILES_URL}/download-url?type=image&file_id=${song.imagePath}`,
          sizes: size,
          type: "image/*",
        })),
      });
      
      const handlers = {
        play: () => audio.play().catch(console.error),
        pause: () => audio.pause(),
        previoustrack: setPreviousSong,
        nexttrack: setNextSong,
        seekto: (details: MediaSessionActionDetails) => {
          if (details.seekTime !== undefined) {
            audio.currentTime = details.seekTime;
          }
        },
      };

      Object.entries(handlers).forEach(([action, handler]) => {
        navigator.mediaSession.setActionHandler(action as MediaSessionAction, handler);
      });
    };

    // Attach event listeners
    const events: [string, EventListenerOrEventListenerObject][] = [
      ["timeupdate", handleTimeUpdate],
      ["loadedmetadata", handleLoadedMetadata],
      ["ended", handleEnded],
      ["play", handlePlay],
      ["pause", handlePause],
      ["loadstart", handleLoadStart],
      ["canplay", handleCanPlay],
      ["waiting", handleWaiting],
      ["seeking", handleSeeking],
      ["seeked", handleSeeked],
      ["error", handleError as EventListenerOrEventListenerObject],
      ["canplaythrough", handleCanPlayThrough],
    ];

    events.forEach(([event, handler]) => audio.addEventListener(event, handler));
    document.addEventListener("keydown", handleKeyDown);
    setupMediaSession();

    return () => {
      isActive = false;
      audio.pause();
      events.forEach(([event, handler]) => audio.removeEventListener(event, handler));
      document.removeEventListener("keydown", handleKeyDown);

      if ("mediaSession" in navigator) {
        navigator.mediaSession.metadata = null;
        ["play", "pause", "nexttrack", "previoustrack", "seekto"].forEach(action => {
          navigator.mediaSession.setActionHandler(action as MediaSessionAction, null);
        });
      }
    };
    // Volume value changes triggers replay, so we cannot add it to main effect deps
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [songUrl, song, setNextSong, setPreviousSong]);

  // After first player render setting isRehydrated value false to keep autoplay
  useEffect(() => {
    if (song && songUrl && isRehydrated) {
      setRehydrated(false);
    }
  }, [song, songUrl, setRehydrated, isRehydrated]);

  return { audioRef, isPlaying, duration, currentTime, isStalled, isSeeking };
}
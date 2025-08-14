"use client";

import { AiFillStepBackward, AiFillStepForward } from "react-icons/ai";
import { BsPauseFill, BsPlayFill } from "react-icons/bs";
import { HiSpeakerWave, HiSpeakerXMark } from "react-icons/hi2"
import {useCallback, useEffect, useRef, useState} from "react";
import { useShallow } from "zustand/shallow";

import { Song } from "@/types/types";

import usePlayer from "@/hooks/usePlayer";

import MediaItem from "./MediaItem";
import LikeButton from "./LikeButton";
import Slider from "./Slider";

type changeSongType = "previous" | "next";

// Helper function to format time
const formatTime = (timeInSeconds: number): string => {
  if (isNaN(timeInSeconds)) return "0:00";
  const minutes = Math.floor(timeInSeconds / 60);
  const seconds = Math.floor(timeInSeconds % 60);
  return `${minutes}:${seconds < 10 ? '0' : ''}${seconds}`;
};

const PlayerContent = ({
  song,
  songUrl
}: {
  song: Song;
  songUrl: string;
}) => {
  const [setNextId, setPreviousId, volume, setVolume] = usePlayer(useShallow(s => [
    s.setNextId,
    s.setPreviousId,
    s.volume,
    s.setVolume,
  ]));
  
  const audioRef = useRef<HTMLAudioElement>(null);
  const [isPlaying, setIsPlaying] = useState(false);
  const [currentTime, setCurrentTime] = useState(0);
  const [duration, setDuration] = useState(0);

  const Icon = isPlaying ? BsPauseFill : BsPlayFill;
  const VolumeIcon = volume === 0 ? HiSpeakerXMark : HiSpeakerWave;

  const progress = duration > 0 ? currentTime / duration : 0;

  // Handle song URL changes
  useEffect(() => {
    if (!audioRef.current) return;

    const audio = audioRef.current;
    audio.src = songUrl;
    audio.volume = volume;

    const updateTime = () => setCurrentTime(audio.currentTime);
    const updateDuration = () => setDuration(audio.duration);
    const handleEnded = () => handleChangeSong("next");
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

  // Play/pause button handler
  const togglePlay = useCallback(() => {
    if (!audioRef.current) return;

    if (isPlaying) {
      audioRef.current.pause();
    } else {
      audioRef.current.play().catch(error => {
        console.log("Play error:", error);
      });
    }
  }, [isPlaying]);
  
  const toggleMute = () => setVolume(volume === 0 ? 1 : 0);

  // Song navigation handler
  const handleChangeSong = (changeType: changeSongType) => {
    if (audioRef.current) {
      audioRef.current.pause();
    }
    if (changeType === "next") {
      setNextId();
    } else {
      setPreviousId();
    }
  };

  // Progress slider callback
  const handleProgressChange = useCallback((values: number[]) => {
    if (audioRef.current) {
      audioRef.current.currentTime = values[0] * duration;
    }
  }, [duration]);

  // Volume slider callback
  const handleVolumeChange = useCallback((values: number[]) => {
    setVolume(values[0]);
  }, [setVolume]);

  return (
    <div className="grid grid-cols-2 md:grid-cols-3 h-full">
      <audio ref={audioRef} preload="auto" />
      {/* Media info section */}
      <div className="flex items-center justify-start pl-2">
        <div className="flex items-center gap-2">
          <MediaItem data={song} />
          <LikeButton songId={song.id} />
        </div>
      </div>

      {/* Mobile controls */}
      <div className="flex md:hidden items-center justify-end pr-4">
        <button
          onClick={togglePlay}
          className="flex items-center justify-center h-10 w-10 rounded-full bg-white"
        >
          <Icon size={30} className="text-black"/>
        </button>
      </div>

      {/* Desktop player controls */}
      <div className="hidden md:flex flex-col items-center justify-center w-full">
        <div className="flex items-center justify-center w-full gap-6">
          <button
            onClick={() => handleChangeSong("previous")}
            className="text-neutral-400 hover:text-white transition-colors focus:outline-none"
            aria-label="Previous song"
          >
            <AiFillStepBackward size={23}/>
          </button>
          <button
            onClick={togglePlay}
            className="flex items-center justify-center h-8 w-8 rounded-full bg-white"
            aria-label={isPlaying ? "Pause" : "Play"}
          >
            <Icon size={24} className="text-black" />
          </button>
          <button
            onClick={() => handleChangeSong("next")}
            className="text-neutral-400 hover:text-white transition-colors"
            aria-label="Next song"
          >
            <AiFillStepForward size={23}/>
          </button>
        </div>

        {/* Progress bar */}
        <div className="flex items-center w-full max-w-2xl gap-2">
          <span className="text-sm text-right tabular-nums text-neutral-400 w-12">
            {formatTime(currentTime)}
          </span>
          <div className="flex-1">
            <Slider
              value={progress}
              onChange={handleProgressChange}
            />
          </div>
          <span className="text-sm text-left tabular-nums text-neutral-400 w-12">
            {formatTime(duration)}
          </span>
        </div>
      </div>

      {/* Volume controls */}
      <div className="hidden md:flex items-cetner justify-end pr-4">
        <div className="flex items-center gap-2 w-32">
          <button
            onClick={toggleMute}
            className="text-neutral-400 hover:text-white transition-colors"
            aria-label={volume === 0 ? "Unmute" : "Mute"}
          >
            <VolumeIcon size={20}/>
          </button>
          <Slider
            value={volume}
            onChange={handleVolumeChange}
          />
        </div>
      </div>
    </div>
  );
};
 
export default PlayerContent;

"use client";

import { useMemo, useCallback } from "react";
import { useShallow } from "zustand/shallow";
import { AiFillStepBackward, AiFillStepForward } from "react-icons/ai";
import { BsPauseFill, BsPlayFill } from "react-icons/bs";
import { HiSpeakerWave, HiSpeakerXMark } from "react-icons/hi2";

import { CLIENT_FILES_URL } from "@/shared/config/api";
import { Slider } from "@/shared/ui";
import { SongListItem } from "@/entities/song/ui";
import { LikeButton } from "@/features/like-button";
import {usePlayerStore} from "@/features/player";
import { useSound, useGetCurrentSong, formatTime } from "../lib";

const Player = () => {
  const [activeId, ids, volume, setNextId, setPreviousId, setVolume] = usePlayerStore(
    useShallow((s) => [
      s.activeId,
      s.ids,
      s.volume,
      s.setNextId,
      s.setPreviousId,
      s.setVolume,
    ])
  );

  const { currentSong, isLoading } = useGetCurrentSong();

  const songUrl = useMemo(() => {
    if (!currentSong) return undefined;
    return `${CLIENT_FILES_URL}/download-url?type=audio&file_id=${currentSong.songPath}`;
  }, [currentSong]);

  const { audioRef, isPlaying, duration, currentTime, isStalled } = useSound(
    currentSong,
    songUrl,
    volume,
    setNextId,
    setPreviousId
  );

  const pause = useCallback(
    () => {
      if (!audioRef.current) return;
      audioRef.current.pause();
    },
    [audioRef]
  );

  const play = useCallback(
    () => {
      if (!audioRef.current) return;
      audioRef.current.play().catch((error: unknown) => {
        console.error("Player error:", error);
      });
    },
    [audioRef]
  )

  // Play/pause button handler
  const togglePlay = useCallback(() => {
    if (isPlaying) {
      pause();
    } else {
      play()
    }
  }, [isPlaying, pause, play]);

  // Progress slider callback
  const handleProgressChange = useCallback(
    (values: number[]) => {
      if (!audioRef.current) {
        return;
      }

      const value = values[0];

      if (value === 1) {
        audioRef.current.currentTime = audioRef.current.duration - 1;
      } else {
        audioRef.current.currentTime = values[0] * audioRef.current.duration;
      }
    },
    [audioRef]
  );

  // Next button handler
  const handleNext = useCallback(
    () => {
      if (audioRef.current) {
        audioRef.current.pause();
      }
      setNextId();
    },
    [audioRef, setNextId]
  );

  const handlePrevious = useCallback(
    () => {
      if (audioRef.current) {
        audioRef.current.pause();
      }
      setPreviousId();
    },
    [audioRef, setPreviousId]
  )

  // Volume slider callback
  const handleVolumeChange = useCallback(
    (values: number[]) => {
      setVolume(values[0]);
    },
    [setVolume]
  );

  // Don't show player if no activeId and no ids
  if (!activeId && ids.length === 0) {
    return null;
  }

  const toggleMute = () => setVolume(volume === 0 ? 1 : 0);

  const Icon = isPlaying ? BsPauseFill : BsPlayFill;
  const VolumeIcon = volume === 0 ? HiSpeakerXMark : HiSpeakerWave;

  const progress = duration > 0 ? currentTime / duration : 0;

  return (
    <div className="fixed bottom-0 bg-black w-full h-[80px]">
      <audio ref={audioRef} preload="auto" />

      <div className="grid grid-cols-2 md:grid-cols-3 h-full">
        {/* Media info section */}
        <div className="flex items-center px-2 w-[300px]">
          {!isLoading && currentSong && (
            <SongListItem song={currentSong}>
              <LikeButton songId={currentSong.id} />
            </SongListItem>
          )}
        </div>

        {/* Mobile controls */}
        <div className="flex md:hidden items-center justify-end pr-4">
          <button
            onClick={togglePlay}
            className="flex items-center justify-center h-10 w-10 rounded-full bg-white cursor-pointer"
            disabled={!currentSong || isLoading}
          >
            <Icon size={30} className="text-black" />
          </button>
        </div>

        {/* Desktop player controls */}
        <div className="hidden md:flex flex-col items-center justify-center w-full">
          <div className="flex items-center justify-center w-full gap-6">
            <button
              onClick={handlePrevious}
              className="text-neutral-400 hover:text-white transition-colors focus:outline-none cursor-pointer"
              aria-label="Previous song"
              disabled={!currentSong || isLoading || isStalled}
            >
              <AiFillStepBackward size={23} />
            </button>
            <button
              onClick={togglePlay}
              className="flex items-center justify-center h-8 w-8 rounded-full bg-white cursor-pointer disabled:opacity-50"
              aria-label={isPlaying ? "Pause" : "Play"}
              disabled={!currentSong || isLoading}
            >
              <Icon size={24} className="text-black" />
            </button>
            <button
              onClick={handleNext}
              className="text-neutral-400 hover:text-white transition-colors cursor-pointer"
              aria-label="Next song"
              disabled={!currentSong || isLoading || isStalled}
            >
              <AiFillStepForward size={23} />
            </button>
          </div>

          {/* Progress bar */}
          <div className="flex items-center w-full max-w-2xl gap-2">
            <span className="text-sm text-right tabular-nums text-neutral-400 w-12">
              {formatTime(currentTime)}
            </span>
            <div className="flex-1">
              <Slider
                value={[progress]}
                onValueCommit={handleProgressChange}
                isLoading={isLoading || isStalled}
                disabled={!currentSong || isLoading || isStalled}
              />
            </div>
            <span className="text-sm text-left tabular-nums text-neutral-400 w-12">
              {formatTime(duration)}
            </span>
          </div>
        </div>

        {/* Volume controls */}
        <div className="hidden md:flex items-center justify-end pr-4">
          <div className="flex items-center gap-2 w-32">
            <button
              onClick={toggleMute}
              className="text-neutral-400 hover:text-white transition-colors cursor-pointer"
              aria-label={volume === 0 ? "Unmute" : "Mute"}
              disabled={!currentSong || isLoading || isStalled}
            >
              <VolumeIcon size={20} />
            </button>
            <Slider
              value={[volume]}
              onValueCommit={handleVolumeChange}
              disabled={!currentSong || isLoading || isStalled}
            />
          </div>
        </div>
      </div>
    </div>
  );
};

export default Player;
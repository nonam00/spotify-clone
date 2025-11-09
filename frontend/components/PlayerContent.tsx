"use client";

import { AiFillStepBackward, AiFillStepForward } from "react-icons/ai";
import { BsPauseFill, BsPlayFill } from "react-icons/bs";
import { HiSpeakerWave, HiSpeakerXMark } from "react-icons/hi2"
import {useCallback} from "react";
import { useShallow } from "zustand/shallow";

import { Song } from "@/types/types";

import usePlayerStorage from "@/hooks/usePlayerStorage";
import useSound from "@/hooks/useSound";

import LikeButton from "./LikeButton";
import Slider from "@/components/ui/Slider";
import MediaItem from "@/components/ui/MediaItem";

// Helper function to format time
function formatTime(timeInSeconds: number): string {
  if (isNaN(timeInSeconds)) return "0:00";
  const minutes = Math.floor(timeInSeconds / 60);
  const seconds = Math.floor(timeInSeconds % 60);
  return `${minutes}:${seconds < 10 ? '0' : ''}${seconds}`;
}

const PlayerContent = ({
  song,
  songUrl
}: {
  song: Song;
  songUrl: string;
}) => {
  const [setNextId, setPreviousId, volume, setVolume] = usePlayerStorage(useShallow(s => [
    s.setNextId,
    s.setPreviousId,
    s.volume,
    s.setVolume,
  ]));

  const { audioRef, isPlaying, duration, currentTime } = useSound(song, songUrl, volume, setNextId, setPreviousId);

  const Icon = isPlaying ? BsPauseFill : BsPlayFill;
  const VolumeIcon = volume === 0 ? HiSpeakerXMark : HiSpeakerWave;

  const progress = duration > 0 ? currentTime / duration : 0;

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
  }, [audioRef, isPlaying]);
  
  const toggleMute = () => setVolume(volume === 0 ? 1 : 0);

  // Progress slider callback
  const handleProgressChange = useCallback((values: number[]) => {
    if (audioRef.current) {
      audioRef.current.currentTime = values[0] * audioRef.current.duration;
    }
  }, [audioRef]);

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
          className="flex items-center justify-center h-10 w-10 rounded-full bg-white cursor-pointer"
        >
          <Icon size={30} className="text-black"/>
        </button>
      </div>

      {/* Desktop player controls */}
      <div className="hidden md:flex flex-col items-center justify-center w-full">
        <div className="flex items-center justify-center w-full gap-6">
          <button
            onClick={setPreviousId}
            className="text-neutral-400 hover:text-white transition-colors focus:outline-none cursor-pointer"
            aria-label="Previous song"
          >
            <AiFillStepBackward size={23}/>
          </button>
          <button
            onClick={togglePlay}
            className="flex items-center justify-center h-8 w-8 rounded-full bg-white cursor-pointer"
            aria-label={isPlaying ? "Pause" : "Play"}
          >
            <Icon size={24} className="text-black" />
          </button>
          <button
            onClick={setNextId}
            className="text-neutral-400 hover:text-white transition-colors cursor-pointer"
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
            className="text-neutral-400 hover:text-white transition-colors cursor-pointer"
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

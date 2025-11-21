import { useCallback, useEffect } from "react";
import { BsPauseFill, BsPlayFill } from "react-icons/bs";

import { Slider } from "@/shared/ui";
import { CLIENT_FILES_URL } from "@/shared/config/api";
import type { Song } from "@/entities/song";
import { useSound } from "../lib";
import { useAudioStore } from "../model";

// Helper function to format time
function formatTime(timeInSeconds: number): string {
  if (isNaN(timeInSeconds) || !isFinite(timeInSeconds)) return "0:00";
  const minutes = Math.floor(timeInSeconds / 60);
  const seconds = Math.floor(timeInSeconds % 60);
  return `${minutes}:${seconds < 10 ? '0' : ''}${seconds}`;
}

type AudioPlayerProps = {
  song: Song;
}

const AudioPlayer = ({ song }: AudioPlayerProps) => {
  const songUrl = `${CLIENT_FILES_URL}/download-url?type=audio&file_id=${song.songPath}`;
  const { audioRef, isPlaying, duration, currentTime, togglePlay, setCurrentTime } = useSound(songUrl);
  const { activeSongId, setActiveSongId } = useAudioStore();

  const Icon = isPlaying ? BsPauseFill : BsPlayFill;
  const progress = duration > 0 ? currentTime / duration : 0;
  const isActive = activeSongId === song.id;

  // Pause this player if another one becomes active
  useEffect(() => {
    if (activeSongId !== null && activeSongId !== song.id && isPlaying && audioRef.current) {
      audioRef.current.pause();
    }
  }, [activeSongId, song.id, isPlaying, audioRef]);

  // Sync activeSongId when playback state changes, but only if we're the active song
  // This prevents circular updates
  useEffect(() => {
    if (isActive && !isPlaying) {
      // If we were active but are now paused, clear activeSongId
      setActiveSongId(null);
    }
  }, [isActive, isPlaying, setActiveSongId]);

  // Enhanced toggle play that stops other players
  const handleTogglePlay = useCallback(() => {
    if (!isPlaying) {
      // If starting to play, set this as active (will stop others)
      setActiveSongId(song.id);
    }
    togglePlay();
  }, [isPlaying, song.id, setActiveSongId, togglePlay]);

  // Progress slider callback
  const handleProgressChange = useCallback((values: number[]) => {
    if (audioRef.current) {
      const newTime = values[0] * duration;
      setCurrentTime(newTime);
    }
  }, [audioRef, duration, setCurrentTime]);

  return (
    <div className="flex items-center gap-x-4 w-full max-w-md">
      <audio ref={audioRef} preload="metadata" />
      
      {/* Play/Pause button */}
      <button
        onClick={handleTogglePlay}
        className="flex items-center justify-center h-12 w-12 rounded-full bg-white hover:opacity-75 cursor-pointer shadow-lg hover:shadow-xl transform hover:scale-105 active:scale-95 transition-all duration-200"
        aria-label={isPlaying ? "Pause" : "Play"}
      >
        <Icon size={22} className="text-black" />
      </button>

      {/* Progress bar and time */}
      <div className="flex-1 flex flex-col gap-y-2">
        <Slider
          value={progress}
          onChange={handleProgressChange}
          max={1}
        />
        <div className="flex items-center justify-between text-xs text-neutral-400 font-mono">
          <span className="font-medium">{formatTime(currentTime)}</span>
          <span className="font-medium">{formatTime(duration)}</span>
        </div>
      </div>
    </div>
  );
};

export default AudioPlayer;


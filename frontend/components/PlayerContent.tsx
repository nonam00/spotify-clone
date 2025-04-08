"use client";

import { AiFillStepBackward, AiFillStepForward } from "react-icons/ai";
import { BsPauseFill, BsPlayFill } from "react-icons/bs";
import { HiSpeakerWave, HiSpeakerXMark } from "react-icons/hi2"
import {useCallback, useEffect, useState} from "react";
import { useShallow } from "zustand/shallow";
import useSound from "use-sound";

import { Song } from "@/types/types";

import usePlayer from "@/hooks/usePlayer";

import MediaItem from "./MediaItem";
import LikeButton from "./LikeButton";
import Slider from "./Slider";

type changeSongType = "previous" | "next";

// parsing current playing time to human-readable format
const getCurrentTimeString = (
  duration: number,
  progress: number
) => {
  const minutes = progress * duration / 60 >> 0;
  const seconds = progress * duration % 60 >> 0;
  return `${minutes}:${seconds >= 10? seconds : `0${seconds}`}`;
}

// parsing track duration to human-readable format
const getDurationString = (duration: number) => {
  const minutes = duration / 60 >> 0;
  const seconds = duration % 60 >> 0;
  return `${minutes}:${seconds >= 10? seconds : `0${seconds}`}`;
}

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
    s.volume, // value for configuring volume from slider
    s.setVolume,
  ]));
  const [isPlaying, setIsPlaying] = useState<boolean>(false);
  const [progress, setProgress] = useState<number>(0); // progress value for the progress bar and time display
  const [delay, setDelay] = useState<NodeJS.Timeout>(); // timeout for updating time and progress bar
  const [currentTimeString, setCurrentTimeString] = useState<string>(""); // variable for displaying current play time of track
  const Icon = isPlaying ? BsPauseFill : BsPlayFill;
  const VolumeIcon = volume === 0 ? HiSpeakerXMark : HiSpeakerWave;

  // songUrl doesn't updates dynamically
  const [play, { pause, sound }] = useSound(
    songUrl, {
      volume: volume,
      onplay: () => {
        setIsPlaying(true);
        setDelay(timeout);
      },
      onend: () => {
        setIsPlaying(false);
        handleChangeSong("next");
      },
      onpause: () => {
        setIsPlaying(false)
        clearTimeout(delay);
      },
      format: ['flac', 'mp3', 'wav','m4a','aac','ogg'],
      html5: true,
    }
  );

  useEffect(() => {
    sound?.play();
    return () => {
      sound?.unload();
    }
  }, [sound]);

  const handlePlay = () => !isPlaying ? play() : pause();

  const toggleMute = () => volume === 0 ? setVolume(1) : setVolume(0);

  // handling player prev and next buttons
  const handleChangeSong = (changeType: changeSongType) => {
    pause();
    clearTimeout(delay);
    if (changeType == "next") {
      setNextId();
    } else if (changeType == "previous") {
      setPreviousId();
    }
  }

  const updateProgress = () => {
    const seek: number = sound?.seek() ?? 0;
    const duration: number = sound?.duration() ?? 1;
    setProgress(seek / duration);
    setCurrentTimeString(getCurrentTimeString(duration, progress));
  }

  const timeout = setTimeout(updateProgress, 100);

  //sliders callbacks
  const setCurrentTimeCallback = useCallback((value: number[]) => {
    clearTimeout(delay);
    if (sound) {
      sound.seek(sound.duration() * value[0])
    }
    setDelay(timeout);
  }, [delay, sound, timeout]);

  const setVolumeCallback = useCallback((value: number[]) => {
    setVolume(value[0]);
  }, [setVolume])

  return (
    <div className="grid grid-cols-2 md:grid-cols-3 h-full" >
      <div className="flex w-full justify-start">
        <div className="flex items-center gap-x-4">
          <MediaItem data={song} />
          <LikeButton songId={song.id} />
        </div>
      </div>
      
      {/* Mobile view */}
      <div className="flex md:hidden col-auto w-full justify-end items-center">
        <div
          onClick={handlePlay}
          className="
            h-10 w-10
            flex
            items-center
            justify-center
            rounded-full
            bg-white
            p-1
            cursor-pointer
          "
        >
          <Icon size={30} className="text-black"/>
        </div>
      </div>
      
      {/* Normal view */}
      <div className="flex flex-col">
        <div
          className="
            hidden
            md:flex
            h-full w-full max-w-[722px]
            justify-center
            items-center
            gap-x-6
          "
        >
          <AiFillStepBackward
            onClick={() => handleChangeSong("previous")}
            size={25}
            className="text-neutral-400 cursor-pointer hover:text-white transition"
          />
          <div
            onClick={handlePlay}
            className="
              flex
              items-center
              justify-center
              h-8 w-8
              p-1
              rounded-full
              bg-white
              cursor-pointer
            "
          >
            <Icon size={25} className="text-black" />
          </div>
          <AiFillStepForward
            onClick={() => handleChangeSong("next")}
            size={25}
            className="text-neutral-400 cursor-pointer hover:text-white transition"
          />
        </div>
        <div className="hidden md:flex md:flex-row items-center gap-x-3">
          <p className="flex align-middle items-center text-center text-sm w-[8%]">
            {sound
              ? currentTimeString
              : ""
            }
          </p>
          <div className="w-[84%]">
            <Slider
              value={progress}
              onChange={setCurrentTimeCallback}
            />
          </div>
          <p className="flex align-middle items-center text-center text-sm w-[8%]">
            {sound
              ? getDurationString(sound.duration() ?? 1)
              : ""
            }
          </p>
        </div>
      </div>

      <div className="hidden md:flex w-full justify-end pr-2">
        <div className="flex items-center gap-x-2 w-[120px]">
          <VolumeIcon
            onClick={toggleMute}
            className="cursor-pointer"
            size={34}
          />
          <Slider
            value={volume}
            onChange={setVolumeCallback}
          />
        </div>
      </div>
    </div>
  );
}
 
export default PlayerContent;

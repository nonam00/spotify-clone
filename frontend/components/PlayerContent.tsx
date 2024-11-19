"use client";

import { AiFillStepBackward, AiFillStepForward } from "react-icons/ai";
import { BsPauseFill, BsPlayFill } from "react-icons/bs";
import { HiSpeakerWave, HiSpeakerXMark } from "react-icons/hi2"
import { useEffect, useState } from "react";
import useSound from "use-sound";

import { Song } from "@/types/types";

import usePlayer from "@/hooks/usePlayer";

import MediaItem from "./MediaItem";
import LikeButton from "./LikeButton";
import Slider from "./Slider";

interface PlayerContentProps {
  song: Song;
  songUrl: string;
}

type changeSongType = "previous" | "next";

const PlayerContent: React.FC<PlayerContentProps> = ({
  song,
  songUrl
}) => {
  const [setNextId, setPreviousId] = usePlayer(s => [
    s.setNextId,
    s.setPreviousId
  ]);
  const [volume, setVolume] = useState<number>(1); // value for configurating volume from slider
  const [isPlaying, setIsPlaying] = useState<boolean>(false);
  const [progress, setProgress] = useState<number>(0); // progress value for the progress bar and time display
  const [delay, setDelay] = useState<NodeJS.Timeout>(); // timeout for updating time and progress bar
  const [currentString, setCurrentString] = useState<string>(""); // variable for displaying current play time of track

  const Icon = isPlaying ? BsPauseFill : BsPlayFill;
  const VolumeIcon = volume === 0 ? HiSpeakerXMark : HiSpeakerWave;

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

  // songUrl doesn't updates dynamically
  const [play, { pause, sound }] = useSound(
    songUrl,
    {
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

  const updateProgress = () => {
    const seek: number = sound?.seek() ?? 0;
    const duration: number = sound?.duration() ?? 1;
    setProgress(seek / duration);
    setCurrentString(getCurrentTimeString(duration));
  }

  const timeout = setTimeout(updateProgress, 100);

  // updating timeout after 
  const setCurrent = (value: number) => {
    clearTimeout(delay);
    if (sound) {
      sound.seek(sound.duration() * value)
    }
    setDelay(timeout);
  }

  // parsing current p;laying time to human-readable format 
  const getCurrentTimeString = (duration: number) => {
    const minutes = progress * duration / 60 >> 0;
    const seconds = progress * duration % 60 >> 0;
    return `${minutes}:${seconds >= 10? seconds : `0${seconds}`}`; 
  }

  // parsing track duration to human-readable format
  const getDurationString = () => {
    const duration: number = sound?.duration() ?? 1;
    const minutes = duration / 60 >> 0;
    const seconds = duration % 60 >> 0;
    return `${minutes}:${seconds >= 10? seconds : `0${seconds}`}`; 
  }

  return (
    <div className="grid grid-cols-2 md:grid-cols-3 h-full" >
      <div className="flex w-full justify-start">
        <div className="flex items-center gap-x-4">
          <MediaItem data={song} />
          <LikeButton songId={song.id} />
        </div>
      </div>
      
      {/* Mobile view */}
      <div
        className="
          flex
          md:hidden
          col-auto
          w-full
          justify-end
          items-center
        "
      >
        <div
          onClick={handlePlay}
          className="
            h-10
            w-10
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
            h-full
            md:flex
            justify-center
            items-center
            w-full
            max-w-[722px]
            gap-x-6
          "
        >
          <AiFillStepBackward
            onClick={() => handleChangeSong("previous")}
            size={25}
            className="
              text-neutral-400
              cursor-pointer
              hover:text-white
              transition
            "
          />
          <div
            onClick={handlePlay}
            className="
              flex
              items-center
              justify-center
              h-8
              w-8
              rounded-full
              bg-white
              p-1
              cursor-pointer
            "
          >
            <Icon size={25} className="text-black" />
          </div>
          <AiFillStepForward
            onClick={() => handleChangeSong("next")}
            size={25}
            className="
              text-neutral-400
              cursor-pointer
              hover:text-white
              transition
            "
          />
        </div>
        <div className="hidden md:flex md:flex-row items-center gap-x-3">
          <p className="flex align-middle items-center text-sm">
            {sound? currentString : ""}
          </p>
          <Slider
            value={progress}
            onChange={(value) => setCurrent(value)}
           />
          <p className="flex align-middle items-center text-sm">
            {sound? getDurationString() : ""}
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
            onChange={(value) => setVolume(value)}
          />
        </div>
      </div>
    </div>
  );
}
 
export default PlayerContent;

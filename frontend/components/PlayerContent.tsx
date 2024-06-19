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

const PlayerContent: React.FC<PlayerContentProps> = ({
  song,
  songUrl
}) => {
  const player = usePlayer();
  const [volume, setVolume] = useState<number>(1);
  const [isPlaying, setIsPlaying] = useState<boolean>(false);
  const [progress, setProgress] = useState<number>(0);
  const [delay, setDelay] = useState<NodeJS.Timeout>();

  const Icon = isPlaying ? BsPauseFill : BsPlayFill;
  const VolumeIcon = volume === 0 ? HiSpeakerXMark : HiSpeakerWave;

  // set next song index
  const onPlayNext = () => {
    if (player.ids.length === 0) {
      return;
    }

    const currentIndex = player.ids.findIndex((id) => id === player.activeId);
    const nextSong = player.ids[currentIndex + 1];

    if (!nextSong) {
      return player.setId(player.ids[0]);
    }

    player.setId(nextSong);
  };

  // set previous song index
  const onPlayPrevious = () => {
    if (player.ids.length === 0) {
      return;
    }

    const currentIndex = player.ids.findIndex((id) => id === player.activeId);
    const previousSong = player.ids[currentIndex - 1];

    if (!previousSong) {
      return player.setId(player.ids[player.ids.length - 1]);
    }

    player.setId(previousSong);
  };

  // TODO: replace with native <audio> component
  // songUrl doesn't updates dinamicly
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
        onPlayNext();
      },
      onpause: () => {
        setIsPlaying(false)
        clearTimeout(delay);
      },
      format: ['flac', 'mp3', 'wav','.m4a','.aac','.ogg'] 
    }
  );

  useEffect(() => {
    sound?.play();
    return () => {
      sound?.unload();
    }
  }, [sound]);

  const handlePlay = () => {
    if (!isPlaying) {
      play();
    } else {
      pause();
    }
  }

  const toggleMute = () => {
    if (volume === 0) {
      setVolume(1);
    } else {
      setVolume(0);
    }
  }

  const timeout: NodeJS.Timeout = setTimeout(() => {
    updateProgress();
 }, 1000);

  const updateProgress = () => {
    const seek: number = sound?.seek() ?? 0;
    const duration: number = sound?.duration() ?? 1;
    setProgress(seek / duration);
  }

  const setCurrent = (value: number) => {
    if(sound) {
      sound.seek(sound.duration() * value)
    }
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
            onClick={onPlayPrevious}
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
            onClick={onPlayNext}
            size={25}
            className="
              text-neutral-400
              cursor-pointer
              hover:text-white
              transition
            "
          />
        </div>
        <div className="hidden md:flex">
          <Slider
            value={progress}
            onChange={(value) => {setCurrent(value)}}
           />
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
"use client";

import useLoadImage from "@/hooks/useLoadImage";
import { Playlist } from "@/types/types";
import Image from "next/image";
import { useRouter } from "next/navigation";
import { FaPlay } from "react-icons/fa";

interface ListItemProps {
  name: string;
  href: string;
  playlist?: Playlist
}

const ListItem: React.FC<ListItemProps> = ({
  name,
  href,
  playlist
}) => {
  const image = useLoadImage(playlist) ?? "/images/playlist.webp";
  const router = useRouter();
  
  const onClick = () => {
    //Add auth before push
    router.push(href);
  }
    

  return ( 
    <button
    onClick={onClick}
      className="
        relative
        group
        flex
        items-center
        rounded-md
        overflow-hidden
        gap-x-4
        bg-neutral-100/10
        hover:bg-neutral-100/20
        transition
        pr-4
      "
    >
      <div className="
        relative
        min-h-[64px]
        min-w-[64px]
      ">
        <Image
          className="object-cover"
          fill
          src={playlist? image : "/images/liked.png"}
          alt="Image"
        />
      </div>
      <p className="font-medium truncate py-5">
        {name}
      </p>
      <div
        className="
          absolute
          transition
          opacity-0
          rounded-full
          flex
          items-center
          justify-center
          bg-green-500
          p-4
          drop-shadow-md
          right-5
          group-hover:opacity-100
          hover:scale-110
        "
      >
        <FaPlay className="text-black"/>
      </div>
    </button>
  );
}
 
export default ListItem;

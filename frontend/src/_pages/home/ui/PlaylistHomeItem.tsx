import Image from "next/image";
import Link from "next/link";
import { FaPlay } from "react-icons/fa";

const PlaylistHomeItem = ({
  name,
  href,
  image,
}: {
  name: string;
  href: string;
  image: string;
}) => {
  return (
    <Link
      href={href}
      prefetch={true}
      className="
        relative group flex items-center pr-4 rounded-md overflow-hidden gap-x-4
        bg-neutral-100/10 hover:bg-neutral-100/20 transition-colors
      "
    >
      <div className="relative min-h-[64px] min-w-[64px]">
        <Image
          className="object-cover"
          fill
          src={image}
          alt={name}
          loading="lazy"
          unoptimized
        />
      </div>
      <p className="font-medium truncate py-5">{name}</p>
      <div className="
        absolute flex items-center justify-center right-5 p-4
        rounded-full drop-shadow-md opacity-0 bg-green-500 group-hover:opacity-100 hover:scale-110 transition
      ">
        <FaPlay className="text-black" />
      </div>
    </Link>
  );
};

export default PlaylistHomeItem;
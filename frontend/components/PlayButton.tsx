import { FaPlay } from "react-icons/fa";

const PlayButton = () => {
  return (
    <button
      className="
        flex items-center p-4
        bg-green-500 drop-shadow-md opacity-0 rounded-full
        translate transtate-1/4
        group-hover:opacity-100 group-hover:translate-y-0
        hover:scale-110
        transition
      "
    >
      <FaPlay className="text-black"/>
    </button>
  );
}
 
export default PlayButton;
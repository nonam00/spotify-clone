import { redirect }  from "next/navigation";

import getPlaylistById from "@/actions/getPlaylistById";
import getSongsByPlaylistId from "@/actions/getSongsByPlaylistId";

import Header from "@/components/Header"
import PlaylistContent from "./components/PlaylistContent";
import PlaylistImage from "./components/PlaylistImage";

export const revalidate = 0;

interface PlaylistProps {
  params: {
    id: string
  }
}

const Playlist = async ({
  params: { id },
}: PlaylistProps) => {
  const playlist = await getPlaylistById(id);
  if (!playlist) {
    redirect("/");
  }
  const songs = await getSongsByPlaylistId(playlist.id);
  
  return (
    <div
      className="
        bg-neutral-900
        rounded-lg
        h-full
        w-full
        overflow-y-auto
      "
    >
      <Header>
        <div className="mt-20">
          <div
            className="
              flex
              flex-col
              md:flex-row
              items-center
              gap-x-5
            "
          >
            <div className="
              relative
              h-32
              w-32
              lg:h-44
              lg:w-44
            ">
              <PlaylistImage
                playlist={playlist}
              />
            </div>
            <div className="
              flex
              flex-col
              gap-y-5
              mt-4
              md:mt-0
            ">
              <h1
                className="
                  text-white
                  text-4xl
                  sm:text-5xl
                  lg:text-7xl
                  font-bold
                "
              >
                {playlist.title}
              </h1>
              <p className="
                flex
                md:box
                font-semibold
                md:text-sm
                justify-center
                md:justify-normal
                text-neutral-300
                md:text-white
              ">
                {playlist.description ?? "No description"}
              </p>
            </div>
          </div>
        </div>
      </Header>
      <PlaylistContent id={id} songs={songs}/>
    </div>
  );
}

export default Playlist;

import getLikedSongsForPlaylist from "@/actions/getLikedSongsForPlaylist";
import getPlaylistById from "@/actions/getPlaylistById";

import Header from "@/components/Header"
import AddContent from "./components/AddContent";
import SearchInput from "@/components/SearchInput";

export const revalidate = 0;

interface SearchProps {
  searchParams: {
    id: string;
    searchString: string;
  }
}

const Add = async ({searchParams}: SearchProps) => {
  const playlist = await getPlaylistById(searchParams.id);
  if (!playlist) {
    throw new Error("No playlist has been found!");
  }
  const songs = await getLikedSongsForPlaylist(playlist.id, searchParams.searchString ?? "");

  return (
    <div
      className="
        bg-neutral-900
        rounded-lg
        h-full
        w-full
        overflow-hidden
        overflow-y-auto
      "
    >
      <Header className="from-bg-neutral-900"> 
        <div className="mb-2 flex flex-col gap-y-6">
          <h1 className="text-white text-3xl font-semibold">
            Select songs from your favorites to add to your playlist
          </h1>
          <SearchInput pageUrl={`/add/?id=${searchParams.id}`} types={false}/>
        </div>
      </Header>
      <AddContent playlistId={playlist.id} songs={songs} />
    </div>
  )
}

export default Add;

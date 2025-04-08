import { redirect } from "next/navigation";
import getLikedSongsForPlaylist from "@/actions/liked/getLikedSongsForPlaylist";
import getPlaylistById from "@/actions/playlists/getPlaylistById";

import Header from "@/components/Header"
import AddContent from "./components/AddContent";
import SearchInput from "@/components/SearchInput";

export const revalidate = 0;

interface AddProps {
  params: Promise<{ id: string }>;
  searchParams: Promise<{ searchString: string }>;
}

const Add = async ({
  params,
  searchParams
}: AddProps) => {
  const [{id}, {searchString}] = [await params, await searchParams];
  const playlist = await getPlaylistById(id);

  if (!playlist) {
    redirect("/");
  }

  const songs = await getLikedSongsForPlaylist(playlist.id, searchString ?? "");

  return (
    <div className="bg-neutral-900 rounded-lg h-full w-full overflow-hidden overflow-y-auto">
      <Header className="from-bg-neutral-900"> 
        <div className="mb-2 flex flex-col gap-y-6">
          <h1 className="text-white text-3xl font-semibold">
            Select songs from your favorites to add to your playlist
          </h1>
          <SearchInput pageUrl={`/playlist/${id}/add/`} types={false}/>
        </div>
      </Header>
      <AddContent playlistId={playlist.id} songs={songs} />
    </div>
  )
}

export default Add;

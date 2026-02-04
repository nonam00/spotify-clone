import { notFound } from "next/navigation";

import {getPlaylistById} from "@/entities/playlist";
import {getLikedSongsForPlaylist} from "@/entities/song";
import { SearchInput } from "@/features/search-input";
import { Header } from "@/widgets/header";
import {AddSongsToPlaylistContent} from "@/_pages/playlist";

export const revalidate = 0;

type AddProps = {
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
    notFound();
  }

  const songs = await getLikedSongsForPlaylist(playlist.id, searchString ?? "");

  return (
    <div className="bg-neutral-900 rounded-lg h-full w-full overflow-hidden overflow-y-auto outline-none">
      <Header className="from-bg-neutral-900"> 
        <div className="mb-2 flex flex-col gap-y-6">
          <h1 className="text-white text-3xl font-semibold">
            Select songs from your favorites to add to your playlist
          </h1>
          <SearchInput pageUrl={`/playlist/${id}/add/`} types={false}/>
        </div>
      </Header>
      <AddSongsToPlaylistContent playlistId={playlist.id} songs={ songs} />
    </div>
  )
}

export default Add;

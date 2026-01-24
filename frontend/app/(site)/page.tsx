import { CLIENT_FILES_URL } from "@/shared/config/api";
import {getSongs} from "@/entities/song";
import {getUserPlaylistsByQuantity} from "@/entities/playlist";
import { Header } from "@/widgets/header";
import { PlaylistHomeItem, PageContent } from "@/_pages/home";

export const revalidate = 0;

export default async function Home() {
  const songs = await getSongs();
  const playlists = await getUserPlaylistsByQuantity(7);

  return (
    <div className="h-full w-full overflow-hidden overflow-y-auto bg-neutral-900 rounded-lg">
      <Header>
        <div className="mb-2">
          <h1 className="text-white text-3xl font-semibold">
            Welcome back
          </h1>
          <div className="grid grid-cols-2 sm:grid-cols-3 xl:grid-cols-3 2xl:grid-cols-4 gap-3 mt-4">
            <PlaylistHomeItem
              name="Liked Songs"
              href="/liked"
              image="/images/liked.png"
            />
            {playlists.map((playlist) => (
              <PlaylistHomeItem
                key = {playlist.id}
                name={playlist.title}
                href={`/playlist/${playlist.id}`}
                image={playlist.imagePath
                  ? `${CLIENT_FILES_URL}/download-url?type=image&file_id=${playlist.imagePath}`
                  : "/images/playlist.webp"}
              />
            ))}
          </div>
        </div>
      </Header>
      <div className="mt-2 mb-7 px-6">
        <h1 className="text-white text-2xl font-semibold">
          Newest songs
        </h1>
        <PageContent songs={songs} />
      </div>
    </div>
  );
}

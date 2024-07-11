import getSongs from "@/actions/getSongs";
import getUserPlaylistsByQuantity from "@/actions/getUserPlaylistsByQuantity";

import Header from "@/components/Header";
import ListItem from "@/components/ListItem";
import PageContent from "./components/PageContent";
import { API_URL } from "@/api/http";

export const revalidate = 0;

export default async function Home() {
  const songs = await getSongs();
  const playlists = await getUserPlaylistsByQuantity(7);

  return (
    <div className="
      bg-neutral-900
      rounded-lg
      h-full
      w-full
      overflow-hidden
      overflow-y-auto
    ">
      <Header>
        <div className="mb-2">
          <h1
            className="
              text-white
              text-3xl 
              font-semibold
            "
          >
            Welcome back
          </h1>
          <div
            className="
              grid 
              grid-cols-2 
              sm:grid-cols-3 
              xl:grid-cols-3 
              2xl:grid-cols-4
              gap-3
              mt-4
            "
          >
            <ListItem
              name="Liked Songs"
              href="/liked"
              image="/images/liked.png"
            />
            {playlists.map((p) => (
              <ListItem
                key = {p.id}
                name={p.title}
                href={`/playlist?id=${p.id}`}
                image={p.imagePath
                  ? `${API_URL}/files/image/${p.imagePath}`
                  : "/images/playlist.webp"}
              />
            ))}
          </div>
        </div>
      </Header>
      <div className="mt-2 mb-7 px-6">
        <div className="flex justify-between items-center">
          <h1 className="text-white text-2xl font-semibold">
            Newest songs
          </h1>
        </div>
        <PageContent songs={songs} />
      </div>
    </div>
  );
}

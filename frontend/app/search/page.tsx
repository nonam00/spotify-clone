import type { SearchType } from "@/entities/song/model";
import {getSongs, getSongsBySearch} from "@/entities/song/api";
import { Header } from "@/widgets/header";
import { SearchInput } from "@/features/search-input";
import { SearchContent } from "@/_pages/songs-search";

type SearchProps = {
  searchParams: Promise<{
    searchString: string;
    type: SearchType;
  }>
}

export const revalidate = 0;

const Search = async ({
  searchParams
}: SearchProps) => {
  const {searchString, type} = await searchParams;

  const handleSearch = async () => {
    return !searchString ? getSongs() : getSongsBySearch(searchString, type);
  };

  const songs = await handleSearch();
  
  return (
    <div className="bg-neutral-900 rounded-lg h-full w-full overflow-hidden overflow-y-auto">
      <Header className="from-bg-neutral-900"> 
        <div className="mb-2 flex flex-col gap-y-6">
          <h1 className="text-white text-3xl font-semibold">
            Search
          </h1>
          <SearchInput pageUrl="/search"/>
        </div>
      </Header>
      <SearchContent songs={songs} />
    </div>
  )
};

export default Search;

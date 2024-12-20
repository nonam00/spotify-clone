import getSongsByAny from "@/actions/songs/search/getSongsByAny"
import getSongsByTitle from "@/actions/songs/search/getSongsByTitle";
import getSongsByAuthor from "@/actions/songs/search/getSongsByAuthor";
import getSongs from "@/actions/songs/getSongs";

import Header from "@/components/Header";
import SearchInput, { SearchType } from "@/components/SearchInput";
import SearchContent from "./components/SearchContent";

interface SearchProps {
  searchParams: Promise<{
    searchString: string;
    type: SearchType;
  }>
}

export const revalidate = 0;

const Search = async ({searchParams}: SearchProps) => { 
  const {searchString, type} = await searchParams;

  const handleSearch = async () => {
    if(!searchString) {
      return getSongs();
    }
    switch (type) {
      case "title":
        return await getSongsByTitle(searchString);
      case "author":
        return await getSongsByAuthor(searchString);
      default:
        return await getSongsByAny(searchString);
    }
  };

  const songs = await handleSearch();
  
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
            Search
          </h1>
          <SearchInput pageUrl="/search" types={true}/>
        </div>
      </Header>
      <SearchContent songs={songs} />
    </div>
  )
};

export default Search;

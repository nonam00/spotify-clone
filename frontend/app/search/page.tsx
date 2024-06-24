import getSongsByAny from "@/actions/getSongsByAny"
import getSongsByTitle from "@/actions/getSongsByTitle";
import getSongsByAuthor from "@/actions/getSongsByAuthor";

import Header from "@/components/Header";
import SearchInput from "@/components/SearchInput";
import SearchContent from "./components/SearchContent";

interface SearchProps {
  searchParams: {
    searchString: string;
    type: string;
  }
};

export const revalidate = 0;

const Search = async ({searchParams}: SearchProps) => { 
  const handleSearch = async () => {
    if(!searchParams.searchString) {
      return [];
    }
    switch (searchParams.type) {
      case '1':
        return await getSongsByTitle(searchParams.searchString);
      case '2':
        return await getSongsByAuthor(searchParams.searchString);
      default:
        return await getSongsByAny(searchParams.searchString);
    }
  }
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

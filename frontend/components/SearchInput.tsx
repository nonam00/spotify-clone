"use client";

import { useRouter } from "next/navigation";
import { useEffect, useState } from "react";
import { twMerge } from "tailwind-merge";

import useDebounce from "@/hooks/useDebounce";
import Input from "./Input";

interface SearchInputProps {
  pageUrl: string
  types?: boolean
}

export type SearchType = "all" | "title" | "author";

const SearchInput: React.FC<SearchInputProps> = ({
  pageUrl,
  types = true
}) => {
  const router = useRouter();
  const [value, setValue] = useState<string>("");
  const debouncedValue = useDebounce<string>(value, 500);
  const [searchType, setSearchType] = useState<SearchType>("all");
  const debouncedType = useDebounce<SearchType>(searchType, 200);

  const nonActiveStyle = "bg-neutral-700 text-white"
  const activeStyle = "bg-white text-black";

  useEffect(() => {
    const query = `?searchString=${debouncedValue}&type=${debouncedType}`;
    if (query != location.search) {
      router.push(pageUrl + query);
    }
  }, [debouncedValue, debouncedType, router, pageUrl]);

  return (
    <div>
      <Input
        placeholder="What do you want to listen to ?"
        value={value}
        onChange={(e) => setValue(e.target.value)}
      />
      {types? 
      <div
        className="
          flex 
          flex-row
          mt-5
          gap-x-0.7
        "
      >
        <button
          onClick={() => {setSearchType("all")}}
          className={twMerge(`
            rounded-full
            py-1.5
            px-5
            mr-2
            text-sm
            flex
            items-center
            justify-center
            hover:opacity-75
            transition
          `, 
            searchType === "all"? activeStyle : nonActiveStyle    
          )}
        >
          All
        </button>
        <button
          onClick={() => {setSearchType("title")}}
          className={twMerge(`
            rounded-full
            py-1.5
            px-5
            mr-2
            text-sm
            flex
            items-center
            justify-center
            hover:opacity-75
            transition
          `, 
            searchType === "title"? activeStyle : nonActiveStyle    
          )} 
        >
          By Title
        </button>
        <button
          onClick={() => {setSearchType("author")}}
          className={twMerge(`
            rounded-full
            py-1.5
            px-5
            mr-2
            text-sm
            flex
            items-center
            justify-center
            hover:opacity-75
            transition
          `, 
            searchType === "author"? activeStyle : nonActiveStyle    
          )} 
        >
          By Author
        </button>
      </div>
      : <></>}
    </div>
  );
}
 
export default SearchInput;

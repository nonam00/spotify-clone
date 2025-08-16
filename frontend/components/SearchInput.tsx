"use client";

import { useRouter } from "next/navigation";
import { useEffect, useState } from "react";
import { twMerge } from "tailwind-merge";

import useDebounce from "@/hooks/useDebounce";
import { SearchType } from "@/types/types";
import Input from "@/components/ui/Input";

const SearchInput = ({
  pageUrl,
  types = true
}: {
  pageUrl: string
  types?: boolean
}) => {
  const router = useRouter();
  
  const [value, setValue] = useState<string>("");
  const debouncedValue = useDebounce<string>(value, 500);
  
  const [searchType, setSearchType] = useState<SearchType>("any");
  const debouncedType = useDebounce<SearchType>(searchType, 200);

  const nonActiveStyle = "bg-neutral-700 text-white"
  const activeStyle = "bg-white text-black";

  useEffect(() => {
    const query = `?searchString=${debouncedValue}&type=${debouncedType}`;
    if (debouncedValue !== "" && query !== location.search) {
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
      {types ?
      <div className="flex flex-row mt-5 gap-x-0.7">
        <button
          onClick={() => {setSearchType("any")}}
          className={twMerge(`
            flex items-center justify-center
            py-1.5 px-5 mr-2
            rounded-full
            text-sm
            hover:opacity-75
            transition
          `, 
            searchType === "any" ? activeStyle : nonActiveStyle
          )}
        >
          All
        </button>
        <button
          onClick={() => {setSearchType("title")}}
          className={twMerge(`
            flex items-center justify-center
            py-1.5 px-5 mr-2
            rounded-full
            text-sm
            hover:opacity-75
            transition
          `, 
            searchType === "title" ? activeStyle : nonActiveStyle
          )} 
        >
          By Title
        </button>
        <button
          onClick={() => {setSearchType("author")}}
          className={twMerge(`
            flex items-center justify-center
            py-1.5 px-5 mr-2
            rounded-full
            text-sm
            hover:opacity-75
            transition
          `, 
            searchType === "author" ? activeStyle : nonActiveStyle
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

"use client";

import qs from "query-string";

import { useRouter } from "next/navigation";
import { useEffect, useState } from "react";
import { twMerge } from "tailwind-merge";

import useDebounce from "@/hooks/useDebounce";

import Input from "./Input";

const SearchInput = () => {
  const router = useRouter();
  const [value, setValue] = useState<string>("");
  const debouncedValue = useDebounce<string>(value, 500);
  const [searchType, setSearchType] = useState(0);
  const debouncedType = useDebounce<number>(searchType, 200);

  const nonActiveStyle = "bg-neutral-700 text-white"
  const activeStyle = "bg-white text-black";

  useEffect(() => {
    const query = {
      searchString: debouncedValue,
      type: debouncedType
    };

    const url = qs.stringifyUrl({
      url: "/search",
      query: query
    });

    router.push(url);
  }, [debouncedValue, debouncedType, router]);

  return (
    <div>
      <Input
        placeholder="What do you want to listen to ?"
        value={value}
        onChange={(e) => setValue(e.target.value)}
      />
      <div
        className="
          flex 
          flex-row
          mt-5
          gap-x-0.7
        "
      >
        <button
          onClick={() => {setSearchType(0)}}
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
            searchType === 0? activeStyle : nonActiveStyle    
          )}
        >
          All
        </button>
        <button
          onClick={() => {setSearchType(1)}}
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
            searchType === 1? activeStyle : nonActiveStyle    
          )} 
        >
          By Title
        </button>
        <button
          onClick={() => {setSearchType(2)}}
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
            searchType === 2? activeStyle : nonActiveStyle    
          )} 
        >
          By Author
        </button>
      </div>
    </div>
  );
}
 
export default SearchInput;

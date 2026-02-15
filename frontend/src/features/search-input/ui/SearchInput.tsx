"use client";

import { twMerge } from "tailwind-merge";
import { Input } from "@/shared/ui";
import {useDebouncedSearch} from "../lib";

const MINIMUM_INPUT_LENGTH = 3;
const MAXIMUM_INPUT_LENGTH = 100;

const SearchInput = ({
  pageUrl,
  types = true,
}: {
  pageUrl: string;
  types?: boolean;
}) => {
  const {
    value,
    setValue,
    searchType,
    setSearchType
  } = useDebouncedSearch(
    pageUrl,
    MINIMUM_INPUT_LENGTH,
    MAXIMUM_INPUT_LENGTH,
  );

  return (
    <div>
      <Input
        placeholder="What do you want to listen to ?"
        value={value}
        onChange={(e) => setValue(e.target.value)}
      />
      {types && (
        <div className="flex flex-row mt-5 gap-x-0.7">
          <button
            onClick={() => {setSearchType("any")}}
            className={twMerge(
              `flex items-center justify-center py-1.5 px-5 mr-2 rounded-full text-sm hover:opacity-75 transition`,
              searchType === "any" ? "bg-white text-black" : "bg-neutral-700 text-white"
            )}
          >
            All
          </button>
          <button
            onClick={() => {setSearchType("title")}}
            className={twMerge(
              `flex items-center justify-center py-1.5 px-5 mr-2 rounded-full text-sm hover:opacity-75 transition`,
              searchType === "title" ? "bg-white text-black" : "bg-neutral-700 text-white"
            )}
          >
            By Title
          </button>
          <button
            onClick={() => {setSearchType("author")}}
            className={twMerge(
              `flex items-center justify-center py-1.5 px-5 mr-2 rounded-full text-sm hover:opacity-75 transition`,
              searchType === "author" ? "bg-white text-black" : "bg-neutral-700 text-white"
            )}
          >
            By Author
          </button>
        </div>
      )}
    </div>
  );
};

export default SearchInput;
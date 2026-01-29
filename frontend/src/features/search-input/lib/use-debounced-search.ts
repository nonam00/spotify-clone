import {useRouter} from "next/navigation";
import {useEffect, useState} from "react";

import { useDebounce } from "@/shared/lib/use-debounce";
import { SearchType } from "@/entities/song";

export function useDebouncedSearch(
  pageUrl: string,
  minimumLength: number
) {
  const router = useRouter();

  const [value, setValue] = useState<string>("");
  const debouncedValue = useDebounce<string>(value, 500);

  const [searchType, setSearchType] = useState<SearchType>("any");
  const debouncedType = useDebounce<SearchType>(searchType, 200);

  useEffect(() => {
    const query = `?searchString=${debouncedValue}&type=${debouncedType}`;
    if (
      debouncedValue.length >= minimumLength &&
      typeof window !== "undefined" &&
      query !== window.location.search
    ) {
      router.push(pageUrl + query);
    }
  }, [debouncedValue, debouncedType, router, pageUrl, minimumLength]);

  return { value, setValue, searchType, setSearchType };
}
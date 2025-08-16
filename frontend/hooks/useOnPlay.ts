import {Song} from "@/types/types";

import usePlayerStorage from "./usePlayerStorage";
import useAuthModal from "./useAuthModal";
import {useUser} from "./useUser";
import { useCallback } from "react";

const useOnPlay = (songs: Song[]) => {
  const { setActiveId, setIds } = usePlayerStorage();
  const authModal = useAuthModal();
  const { isAuth } = useUser();

  return useCallback((id: string) => {
    if (!isAuth) {
      return authModal.onOpen();
    }

    setActiveId(id);
    setIds(songs.map((song) => song.id));
  }, [authModal, isAuth, setActiveId, setIds, songs]);
}

export default useOnPlay;


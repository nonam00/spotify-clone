import {Song} from "@/types/types";

import usePlayer from "./usePlayer";
import useAuthModal from "./useAuthModal";
import {useUser} from "./useUser";

const useOnPlay = (songs: Song[]) => {
  const { setId, setIds } = usePlayer();
  const authModal = useAuthModal();
  const { isAuth } = useUser();

  return (id: string) => {
    if (!isAuth) {
      return authModal.onOpen();
    }

    setId(id);
    setIds(songs.map((song) => song.id));
  };
}

export default useOnPlay;


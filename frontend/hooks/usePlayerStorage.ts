import { create } from "zustand";

interface PlayerStore {
  ids: string[];
  activeId?: string;
  volume: number;
  setActiveId: (id: string) => void;
  setIds: (ids: string[]) => void;
  setNextId: () => void;
  setPreviousId: () => void;
  setVolume: (value: number) => void;
  reset: () => void;
}

const usePlayerStorage = create<PlayerStore>((set, get) => ({
  ids: [],
  activeId: undefined,
  volume: 1,
  setActiveId: (id: string) => set({ activeId: id }),
  setIds: (ids: string[]) => set({ ids: ids }),
  setNextId: () => {
    const ids = get().ids;

    if (ids.length === 0) {
      return;
    }

    const currentIndex = ids.findIndex((id) => id === get().activeId);
    const previousId = ids[currentIndex + 1];

    return get().setActiveId(previousId ?? ids[0]);
  },
  setPreviousId: () => {
    const ids = get().ids;

    if (ids.length === 0) {
      return;
    }

    const currentIndex = ids.findIndex((id) => id === get().activeId);
    const previousId = ids[currentIndex - 1];

    return get().setActiveId(previousId ?? ids[ids.length - 1]);

  },
  setVolume: (value: number) => set({volume: value}),
  reset: () => set({ ids: [], activeId: undefined, volume: 1})
}));

export default usePlayerStorage;

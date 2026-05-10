import { useEffect, useMemo, memo, useCallback } from "react";
import { useShallow } from "zustand/shallow";
import { Modal } from "@/shared/ui";
import { usePlayerStore } from "../model";

type LyricsModalProps = {
  currentTime: number;
  duration: number;
  onSeekTo: (time: number) => void;
  isOpen: boolean;
  onClose: () => void;
};

const LyricsLine = memo(function LyricsLine({
  text,
  isActive,
  onClick,
}: {
  text: string;
  isActive: boolean;
  onClick: () => void;
}) {
  return (
    <p
      onClick={onClick}
      className={`cursor-pointer py-2 px-3 rounded-lg transition-colors text-5xl font-bold ${
        isActive
          ? "text-green-500 bg-green-700/20"
          : "text-gray-400 hover:text-white"
      }`}
    >
      {text}
    </p>
  );
});

export function LyricsModal({
  currentTime,
  onSeekTo,
  isOpen,
  onClose,
}: LyricsModalProps) {
  const { song, lyrics, fetchLyrics } = usePlayerStore(useShallow((s) => ({
    song: s.currentSong,
    lyrics: s.currentLyrics,
    fetchLyrics: s.fetchCurrentSongLyrics,
  })));

  // Fetch lyrics when songId changes
  useEffect(() => {
    if (!song) return;
    fetchLyrics().catch(console.error);
  }, [fetchLyrics, song]);

  // Find active segment based on currentTime
  const activeIndex = useMemo(() => {
    if (lyrics.length === 0 || currentTime == null) return -1;
    return lyrics.findIndex(
      (seg) => currentTime >= seg.start && currentTime < seg.end
    );
  }, [currentTime, lyrics]);

  const handleLineClick = useCallback((start: number) => {
    onSeekTo(start);
  }, [onSeekTo]);

  const onChange = useCallback((open: boolean) => {
    if (!open) {
      onClose();
    }
  }, [onClose]);

  const setActiveRef = useCallback((node: HTMLDivElement | null) => {
    if (node) {
      node.scrollIntoView({ behavior: 'smooth', block: 'center' });
    }
  }, []);

  return (
    <Modal
      isOpen={isOpen}
      onChange={onChange}
      title="Lyrics"
      description={`${song?.title} - ${song?.author}`}
      className="md:w-[75vw] text-xl"
    >
      <div className="h-[75vh] overflow-y-scroll space-y-5 scrollbar-thin scrollbar-thumb-neutral-600 pb-5">
        {(lyrics.length === 0) ? (
          <p className="text-gray-500 text-3xl text-center">
            No lyrics available
          </p>
        ) : (
          lyrics.map((segment, index) => (
          <div
            key={segment.order}
            ref={index === activeIndex ? setActiveRef : undefined}
          >
            <LyricsLine
              text={segment.text}
              isActive={index === activeIndex}
              onClick={() => handleLineClick(segment.start)}
            />
          </div>
        )))}
      </div>
    </Modal>
  );
}
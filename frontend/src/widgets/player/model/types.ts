import {Song} from "@/entities/song";

export type SongCache = Record<string, Song>;

export type LyricsSegment = {
  start: number;
  end: number;
  text: string;
  order: number;
}
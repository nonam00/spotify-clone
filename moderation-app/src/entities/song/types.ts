export type Song = {
  id: string;
  author: string;
  title: string;
  songPath: string;
  imagePath: string;
}

export type SongListVm = {
  songs: Song[];
}


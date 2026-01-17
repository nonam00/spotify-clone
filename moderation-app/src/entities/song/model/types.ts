export type Song = {
  id: string;
  author: string;
  title: string;
  songPath: string;
  imagePath: string;
  isPublished: boolean;
  createdAt: string;
}

export type SongListVm = {
  songs: Song[];
}
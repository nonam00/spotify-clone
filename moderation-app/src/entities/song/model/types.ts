export type Song = {
  id: string;
  author: string;
  title: string;
  audioPath: string;
  imagePath: string;
  isPublished: boolean;
  containsExplicitContent: boolean;
  createdAt: string;
}

export type SongListVm = {
  songs: Song[];
}
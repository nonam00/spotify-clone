export type Song = {
  id: string;
  author: string;
  title: string;
  audioPath: string;
  imagePath: string;
  containsExplicitContent: boolean;
}

export type SearchType = "any" | "title" | "author";
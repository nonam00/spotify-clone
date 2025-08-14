export interface Song {
  id: string;
  author: string;
  title: string;
  songPath: string;
  imagePath: string;
}

export interface UserDetails {
  email: string;
  fullName: string | null;
}

export interface Playlist {
  id: string;
  title: string;
  description?: string;
  imagePath?: string;
}

// for modal components
export interface ModalStore {
  isOpen: boolean;
  onOpen: () => void;
  onClose: () => void;
}

export type SearchType = "any" | "title" | "author";
export type AuthSubmitType = "login" | "register";

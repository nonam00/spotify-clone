export interface Song {
  id: string;
  author: string;
  title: string;
  songPath: string;
  imagePath: string;
};

export interface UserDetails {
  email: string;
  fullName: string | null;
  avatarUrl: string | null;
  paymentMethod: string | null;
};

export interface Playlist {
  id: string;
  title: string;
  description?: string;
  imagePath?: string;
};

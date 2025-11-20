export interface ManagedUser {
  id: string;
  email: string;
  fullName: string;
  isActive: boolean;
  createdAt: string;
  uploadedSongsCount: number;
}

export interface UserListVm {
  users: ManagedUser[];
}


export type ManagedUser = {
  id: string;
  email: string;
  fullName: string;
  isActive: boolean;
  createdAt: string;
  uploadedSongsCount: number;
}

export type UserListVm = {
  users: ManagedUser[];
}
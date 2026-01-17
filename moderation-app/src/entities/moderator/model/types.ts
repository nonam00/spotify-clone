export type ModeratorInfo = {
  id: string;
  email: string;
  fullName: string;
  isActive: boolean;
  permissions: ModeratorPermissions;
}

export type ModeratorPermissions = {
  canManageUsers: boolean;
  canManageContent: boolean;
  canViewReports: boolean;
  canManageModerators: boolean;
}

export type ModeratorSummary = {
  id: string;
  email: string;
  fullName: string;
  isActive: boolean;
  createdAt: string;
  permissions: ModeratorPermissions;
}

export type ModeratorListVm = {
  moderators: ModeratorSummary[];
}

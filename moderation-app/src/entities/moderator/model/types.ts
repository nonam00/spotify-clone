export interface ModeratorPermissions {
  canManageUsers: boolean;
  canManageContent: boolean;
  canViewReports: boolean;
  canManageModerators: boolean;
}

export interface ModeratorSummary {
  id: string;
  email: string;
  fullName: string;
  isActive: boolean;
  createdAt: string;
  permissions: ModeratorPermissions;
}

export interface ModeratorListVm {
  moderators: ModeratorSummary[];
}


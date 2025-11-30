import { useEffect } from "react";

import { Box, Button } from "@/shared/ui";
import type { ModeratorPermissions } from "@/entities/moderator";
import { useConfirmModalStore } from "@/features/confirm-modal";
import { useAuthStore } from "@/features/auth";
import { useModeratorsStore } from "../model";

const permissionLabels: Record<keyof ModeratorPermissions, string> = {
  canManageContent: "Content",
  canManageUsers: "Users",
  canViewReports: "Reports",
  canManageModerators: "Moderators",
};

const ModeratorsTable = () => {
  const { moderators, isLoading, error, fetchModerators, updatePermissions, updateStatus } = useModeratorsStore();
  const { onOpen } = useConfirmModalStore();
  const { user } = useAuthStore();

  useEffect(() => {
    fetchModerators().catch(() => undefined);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const handlePermissionToggle = async (
    moderatorId: string,
    permissionKey: keyof ModeratorPermissions,
    value: boolean
  ) => {
    const moderator = moderators.find((item) => item.id === moderatorId);
    if (!moderator) return;

    await updatePermissions(moderatorId, {
      canManageUsers: permissionKey === "canManageUsers" ? value : moderator.permissions.canManageUsers,
      canManageContent: permissionKey === "canManageContent" ? value : moderator.permissions.canManageContent,
      canViewReports: permissionKey === "canViewReports" ? value : moderator.permissions.canViewReports,
      canManageModerators:
        permissionKey === "canManageModerators" ? value : moderator.permissions.canManageModerators,
    }).catch(() => undefined);
  };

  const handleStatusChange = (moderatorId: string, isActive: boolean, fullName: string) => {
    const title = isActive ? "Activate moderator?" : "Deactivate moderator?";
    const description = isActive
      ? `Allow ${fullName || "moderator"} to access the platform.`
      : `This moderator will lose access immediately.`;

    onOpen(title, description, async () => {
      await updateStatus(moderatorId, isActive);
    });
  };

  if (isLoading && moderators.length === 0) {
    return (
      <Box className="p-8">
        <div className="flex flex-col items-center gap-4">
          <div className="animate-spin rounded-full h-10 w-10 border-4 border-emerald-500 border-t-transparent" />
          <p className="text-white text-base font-medium">Loading moderators…</p>
        </div>
      </Box>
    );
  }

  if (error) {
    return (
      <Box className="p-6">
        <div className="flex flex-col items-center gap-3">
          <p className="text-red-400 font-semibold">{error}</p>
          <Button onClick={() => fetchModerators()} className="bg-emerald-500 hover:bg-emerald-600 text-white">
            Try again
          </Button>
        </div>
      </Box>
    );
  }

  if (moderators.length === 0) {
    return (
      <Box className="p-8">
        <div className="text-center space-y-2">
          <p className="text-white text-lg font-semibold">No moderators yet</p>
          <p className="text-neutral-400 text-sm">Invite teammates to share moderation workload.</p>
        </div>
      </Box>
    );
  }

  return (
    <Box className="p-0 overflow-hidden">
      <div className="overflow-x-auto">
        <table className="min-w-full divide-y divide-neutral-800 text-sm">
          <thead className="bg-neutral-900/80">
            <tr>
              <th className="px-6 py-3 text-left font-semibold text-neutral-300 uppercase tracking-wider">Moderator</th>
              <th className="px-6 py-3 text-left font-semibold text-neutral-300 uppercase tracking-wider">Status</th>
              <th className="px-6 py-3 text-left font-semibold text-neutral-300 uppercase tracking-wider">Permissions</th>
              <th className="px-6 py-3 text-left font-semibold text-neutral-300 uppercase tracking-wider">Created</th>
              <th className="px-6 py-3 text-right font-semibold text-neutral-300 uppercase tracking-wider">Actions</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-neutral-800">
            {moderators.map((moderator) => {
              const isSelf = user?.id === moderator.id;
              return (
                <tr key={moderator.id} className="hover:bg-neutral-900/40 transition-colors">
                  <td className="px-6 py-4">
                    <p className="text-white font-medium">{moderator.fullName || "—"}</p>
                    <p className="text-neutral-400 text-xs">{moderator.email}</p>
                  </td>
                  <td className="px-6 py-4">
                    <span
                      className={`inline-flex items-center px-3 py-1 rounded-full text-xs font-semibold ${
                        moderator.isActive
                          ? "bg-emerald-500/10 text-emerald-300 border border-emerald-500/40"
                          : "bg-red-500/10 text-red-300 border border-red-500/40"
                      }`}
                    >
                      {moderator.isActive ? "Active" : "Disabled"}
                    </span>
                  </td>
                  <td className="px-6 py-4">
                    <div className="flex flex-wrap gap-3">
                      {(Object.keys(permissionLabels) as (keyof ModeratorPermissions)[]).map((key) => (
                        <label key={key} className="flex items-center gap-2 text-neutral-300 text-xs">
                          <input
                            type="checkbox"
                            checked={moderator.permissions[key]}
                            onChange={(event) =>
                              handlePermissionToggle(moderator.id, key, event.target.checked)}
                            className="h-4 w-4 rounded border-neutral-600 bg-neutral-800 text-emerald-500 focus:ring-emerald-500"
                            disabled={isLoading || !moderator.isActive || isSelf}
                          />
                          {permissionLabels[key]}
                        </label>
                      ))}
                    </div>
                  </td>
                  <td className="px-6 py-4 text-neutral-300">
                    {new Date(moderator.createdAt).toLocaleDateString()}
                  </td>
                  <td className="px-6 py-4 text-right">
                    <Button
                      disabled={isLoading || isSelf}
                      onClick={() => handleStatusChange(moderator.id, !moderator.isActive, moderator.fullName || moderator.email)}
                      className={`px-4 py-2 text-xs ${
                        moderator.isActive
                          ? "bg-red-500 hover:bg-red-600 text-white"
                          : "bg-emerald-500 hover:bg-emerald-600 text-white"
                      } disabled:opacity-60`}
                    >
                      {moderator.isActive ? "Disable" : "Activate"}
                    </Button>
                  </td>
                </tr>
              );
            })}
          </tbody>
        </table>
      </div>
    </Box>
  );
};

export default ModeratorsTable;
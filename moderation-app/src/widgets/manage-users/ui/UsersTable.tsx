import { useCallback, useEffect } from "react";
import { useShallow } from "zustand/react/shallow";

import { Box, Button } from "@/shared/ui";
import type { ManagedUser } from "@/entities/user";
import { useConfirmModalStore } from "@/features/confirm-modal";
import { useUserSongsModalStore, useUsersStore } from "../model";
import UserSongsModal from "./UserSongsModal";

const UsersTable = () => {
  const { users, isLoading, error, fetchUsers, activateUser, deactivateUser, } = useUsersStore(
    useShallow((s) => ({
      users: s.users,
      isLoading: s.isLoading,
      error: s.error,
      fetchUsers: s.fetchUsers,
      activateUser: s.activateUser,
      deactivateUser: s.deactivateUser,
    }))
  );

  const onOpen = useConfirmModalStore(useShallow((s) => s.onOpen));
  const openSongsModal = useUserSongsModalStore(useShallow((s) => s.open));

  const handleViewSongs = useCallback((user: ManagedUser) => {
    if (user.uploadedSongsCount === 0) {
      return;
    }
    openSongsModal(user);
  }, [openSongsModal]);

  useEffect(() => {
    fetchUsers().catch(() => undefined);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const handleStatusChange = useCallback((userId: string, isActive: boolean, email: string) => {
    const title = isActive
      ? "Activate user?"
      : "Suspend user?";

    const description = isActive
      ? `Allow ${email} to access their library.`
      : `User ${email} will be logged out and blocked.`;

    const action = isActive
      ? async () => await activateUser(userId)
      : async () => await deactivateUser(userId);

    onOpen(title, description, action);
  }, [activateUser, deactivateUser, onOpen]);

  if (isLoading && users.length === 0) {
    return (
      <Box className="p-8">
        <div className="flex flex-col items-center gap-4">
          <div className="h-10 w-10 rounded-full border-4 border-emerald-500 border-t-transparent animate-spin" />
          <p className="text-white text-base font-medium">Loading users…</p>
        </div>
      </Box>
    );
  }

  if (error) {
    return (
      <Box className="p-6">
        <div className="flex flex-col items-center gap-3">
          <p className="text-red-400 font-semibold">{error}</p>
          <Button onClick={() => fetchUsers()} className="bg-emerald-500 hover:bg-emerald-600 text-white">
            Try again
          </Button>
        </div>
      </Box>
    );
  }

  if (users.length === 0) {
    return (
      <Box className="p-8">
        <div className="text-center space-y-2">
          <p className="text-white text-lg font-semibold">No users found</p>
          <p className="text-neutral-400 text-sm">Once users sign up they will appear here.</p>
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
              <th className="px-6 py-3 text-left font-semibold text-neutral-300 uppercase tracking-wider">User</th>
              <th className="px-6 py-3 text-left font-semibold text-neutral-300 uppercase tracking-wider">Status</th>
              <th className="px-6 py-3 text-left font-semibold text-neutral-300 uppercase tracking-wider">Registered</th>
              <th className="px-6 py-3 text-left font-semibold text-neutral-300 uppercase tracking-wider">Uploads</th>
              <th className="px-6 py-3 text-right font-semibold text-neutral-300 uppercase tracking-wider">Actions</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-neutral-800">
            {users.map((user) => (
              <tr key={user.id} className="hover:bg-neutral-900/40 transition-colors">
                <td className="px-6 py-4">
                  <p className="text-white font-medium">{user.fullName || "—"}</p>
                  <p className="text-neutral-400 text-xs">{user.email}</p>
                </td>
                <td className="px-6 py-4">
                  <span
                    className={`inline-flex items-center px-3 py-1 rounded-full text-xs font-semibold ${
                      user.isActive
                        ? "bg-emerald-500/10 text-emerald-300 border border-emerald-500/40"
                        : "bg-red-500/10 text-red-300 border border-red-500/40"
                    }`}
                  >
                    {user.isActive ? "Active" : "Suspended"}
                  </span>
                </td>
                <td className="px-6 py-4 text-neutral-300">{new Date(user.createdAt).toLocaleDateString()}</td>
                <td className="px-6 py-4 text-neutral-200">
                  <span className="font-semibold text-white">{user.uploadedSongsCount}</span>
                  <Button
                    onClick={() => handleViewSongs(user)}
                    disabled={isLoading || user.uploadedSongsCount === 0}
                    className="ml-3 bg-neutral-800 hover:bg-neutral-700 text-white px-3 py-1 text-xs disabled:opacity-40"
                  >
                    View songs
                  </Button>
                </td>
                <td className="px-6 py-4 text-right">
                  <Button
                    disabled={isLoading}
                    onClick={() => handleStatusChange(user.id, !user.isActive, user.email)}
                    className={`px-4 py-2 text-xs ${
                      user.isActive ? "bg-red-500 hover:bg-red-600" : "bg-emerald-500 hover:bg-emerald-600"
                    } text-white disabled:opacity-60`}
                  >
                    {user.isActive ? "Suspend" : "Activate"}
                  </Button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
      <UserSongsModal />
    </Box>
  );
};

export default UsersTable;
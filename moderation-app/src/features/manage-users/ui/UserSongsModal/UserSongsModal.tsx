import { Modal, Box, Button, MediaItem } from "@/shared/ui";
import { useUserSongsModalStore } from "@/shared/store/userSongsModalStore.ts";
import { useConfirmModalStore } from "@/shared/store/confirmModalStore.ts";
import AudioPlayer from "@/features/moderate-song/ui/AudioPlayer/AudioPlayer.tsx";

const UserSongsModal = () => {
  const { isOpen, user, songs, isLoading, error, close, unpublishSong, unpublishingSongId } = useUserSongsModalStore();
  const { onOpen } = useConfirmModalStore();

  if (!isOpen || !user) {
    return null;
  }

  const title = user.fullName ? `${user.fullName}'s uploads` : `${user.email}'s uploads`;

  const handleUnpublish = (songId: string, songTitle: string) => {
    onOpen(
      "Unpublish this song?",
      `Are you sure you want to unpublish "${songTitle}"? This song will not be available to users`,
      async () => {
        await unpublishSong(songId);
      }
    );
  };

  return (
    <Modal
      isOpen={isOpen}
      onChange={(open) => {
        if (!open) {
          close();
        }
      }}
      title={title}
      description={`Total uploads: ${user.uploadedSongsCount}`}
      className="md:max-w-3xl"
    >
      {isLoading ? (
        <div className="flex items-center justify-center py-10">
          <div className="flex flex-col items-center gap-3">
            <div className="h-10 w-10 border-4 border-emerald-500 border-t-transparent rounded-full animate-spin" />
            <p className="text-white text-sm">Loading songs…</p>
          </div>
        </div>
      ) : error ? (
        <Box className="p-6 bg-red-500/10 border-red-500/40 text-red-200">
          <p className="text-sm">{error}</p>
        </Box>
      ) : songs.length === 0 ? (
        <Box className="p-6 text-center space-y-2">
          <p className="text-white font-semibold text-lg">No uploaded songs</p>
          <p className="text-neutral-300 text-sm">This user has not uploaded any tracks yet.</p>
        </Box>
      ) : (
        <div className="space-y-4 max-h-[60vh] overflow-auto pr-2">
          {songs.map((song) => (
            <Box key={song.id} className="p-4 space-y-4 bg-neutral-900/80 border border-neutral-800 rounded-xl">
              <div className="flex flex-col gap-4 md:flex-row md:items-center">
                <div className="flex-1 min-w-0">
                  <MediaItem data={song} />
                </div>
                <div className="flex flex-col items-start md:items-end gap-2">
                  <p className="text-neutral-500 text-xs">
                    {new Date(song.createdAt).toLocaleString()} · {song.isPublished ? "Published" : "Awaiting review"}
                  </p>
                  <Button
                    onClick={() => handleUnpublish(song.id, song.title)}
                    disabled={unpublishingSongId === song.id || !song.isPublished}
                    className="bg-red-500 hover:bg-red-600 text-white px-4 py-2 text-xs disabled:opacity-50"
                  >
                    {unpublishingSongId === song.id ? "Unpublishing…" : "Unpublish"}
                  </Button>
                </div>
              </div>
              <div className="border-t border-neutral-800 pt-4">
                <AudioPlayer song={song} />
              </div>
            </Box>
          ))}
        </div>
      )}
    </Modal>
  );
};

export default UserSongsModal;


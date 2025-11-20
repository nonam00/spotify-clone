import { Header } from "@/widgets/header";
import { SongList } from "@/widgets/song-list";

const ModeratePage = () => {
  return (
    <div className="h-full flex flex-col bg-black">
      <Header />
      <main className="flex-1 overflow-auto p-6 md:p-8">
        <SongList />
      </main>
    </div>
  );
};

export default ModeratePage;


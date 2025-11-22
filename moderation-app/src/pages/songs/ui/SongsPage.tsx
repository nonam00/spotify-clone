import { Header, NavigationTabs } from "@/widgets/header";
import { SongList } from "@/widgets/moderate-songs";

const SongsPage = () => {
  return (
    <div className="h-full flex flex-col bg-black">
      <Header title="Songs moderation" description="Review and publish community uploads">
        <NavigationTabs />
      </Header>
      <main className="flex-1 overflow-auto p-6 md:p-8">
        <SongList />
      </main>
    </div>
  );
};

export default SongsPage;
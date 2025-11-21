import { Header, NavigationTabs } from "@/widgets/header/ui";
import { CreateModeratorForm, ModeratorsTable } from "@/widgets/manage-moderators/ui";

const ModeratorsPage = () => {
  return (
    <div className="h-full flex flex-col bg-black">
      <Header title="Moderators" description="Manage teammate access and permissions">
        <NavigationTabs />
      </Header>
      <main className="flex-1 overflow-auto p-6 md:p-8 space-y-6">
        <CreateModeratorForm />
        <ModeratorsTable />
      </main>
    </div>
  );
};

export default ModeratorsPage;
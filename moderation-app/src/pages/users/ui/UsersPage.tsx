import { Header, NavigationTabs } from "@/widgets/header/ui";
import { UsersTable } from "@/widgets/manage-users/ui";

const UsersPage = () => {
  return (
    <div className="h-full flex flex-col bg-black">
      <Header title="Users" description="Activate or suspend listener accounts">
        <NavigationTabs />
      </Header>
      <main className="flex-1 overflow-auto p-6 md:p-8">
        <UsersTable />
      </main>
    </div>
  );
};

export default UsersPage;
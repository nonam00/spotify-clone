import { NavLink } from "react-router-dom";
import { useAuthStore } from "@/features/auth";

const navItems = [
  { to: "/", label: "Songs", permission: "canManageContent" as const },
  { to: "/moderators", label: "Moderators", permission: "canManageModerators" as const },
  { to: "/users", label: "Users", permission: "canManageUsers" as const },
];

const NavigationTabs = () => {
  const { hasPermission } = useAuthStore();
  const availableItems = navItems.filter((item) => hasPermission(item.permission));

  if (availableItems.length === 0) {
    return null;
  }

  return (
    <div className="flex flex-wrap gap-3 mt-6">
      {availableItems.map((item) => (
        <NavLink
          key={item.to}
          to={item.to}
          className={({ isActive }) =>
            `
            px-4 py-2 rounded-full text-sm font-medium transition-all
            ${isActive ? "bg-white text-emerald-700 shadow-lg" : "bg-emerald-900/40 text-white/80 hover:bg-emerald-900/60"}
          `
          }
        >
          {item.label}
        </NavLink>
      ))}
    </div>
  );
};

export default NavigationTabs;


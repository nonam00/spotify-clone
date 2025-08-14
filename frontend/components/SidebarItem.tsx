import Link from "next/link";
import { IconType } from "react-icons";
import { twMerge } from "tailwind-merge";

interface SidebarItemProps {
  icon: IconType;
  label: string;
  active?: boolean;
  href: string;
}

const SidebarItem: React.FC<SidebarItemProps> = ({
  icon: Icon,
  label,
  active,
  href
 }) => {
  return (
    <Link
      href={href}
      className={twMerge(`
        flex flex-row items-center
        h-auto w-full py-1 gap-x-4
        text-md font-medium text-neutral-400
        cursor-pointer hover:text-white transition-colors 
      `,
        active && "text-white"
      )}
    >
      <Icon size={26} />
      <p className="truncate w-full">{label}</p>
    </Link>
  );
}
 
export default SidebarItem;
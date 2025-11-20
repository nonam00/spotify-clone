import { twMerge } from "tailwind-merge";

// Placed in widgets for future functions
const Header = ({
  children,
  className
}: Readonly<{
  children?: React.ReactNode;
  className?: string;
}>) => {
  return (
    <div className={twMerge(`
      h-fit 
      bg-gradient-to-b from-emerald-800
      p-8
      shadow-2xl
    `, className)}>
      <div className="w-full mb-4">
        <h1 className="text-4xl font-bold text-white mb-3">
          Songs moderation
        </h1>
        <p className="text-neutral-300 text-lg">Manage unpublished songs</p>
      </div>
      {children}
    </div>
  );
};

export default Header;


import { twMerge } from "tailwind-merge";

const Box = ({
  children,
  className
}: Readonly<{
  children: React.ReactNode;
  className?: string;
}>) => {
  return (
    <div className={twMerge(`
      bg-neutral-900
      rounded-xl
      border border-neutral-700/30
      shadow-xl
      h-fit w-full
    `, className)}>
      {children}
    </div>
  );
};

export default Box;


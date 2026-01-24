import { twMerge } from "tailwind-merge";

export function Box({children, className}: Readonly<{
  children: React.ReactNode;
  className?: string;
}>) {
  return (
    <div className={twMerge(`bg-neutral-900 rounded-lg h-fit w-full`, className)}>
      {children}
    </div>
  );
}
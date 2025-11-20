import { forwardRef } from "react";
import { twMerge } from "tailwind-merge";

type ButtonProps = React.ButtonHTMLAttributes<HTMLButtonElement>;

const Button = forwardRef<HTMLButtonElement, ButtonProps>(({
  className,
  children,
  disabled,
  type = "button",
  ...props
}, ref) => {
  return (
    <button
      type={type}
      className={twMerge(`
        rounded-lg px-4 py-2.5 text-sm font-semibold
        cursor-pointer disabled:cursor-not-allowed disabled:opacity-50
        transition-all duration-200
        transform hover:scale-[1.02] active:scale-[0.98]
        shadow-md hover:shadow-lg
        focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-offset-neutral-900
      `,
        className
      )}
      disabled={disabled}
      ref={ref}
      {...props}
    >
      {children}
    </button>
  );
});

Button.displayName = "Button";
export default Button;

